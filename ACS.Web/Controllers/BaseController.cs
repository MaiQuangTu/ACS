using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace ACS.Web.Controllers
{
	using ACS.Web.Common;
	using Microsoft.AspNetCore.Hosting;
	using OfficeOpenXml;
	using OfficeOpenXml.Table;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.OleDb;
	using System.IO;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;

	public abstract class BaseController : Controller
    {
        protected readonly IWebHostEnvironment Environment;
        private readonly ILogger Logger;

		protected BaseController(IWebHostEnvironment environment, ILogger<BaseController> logger)
        {
            this.Environment = environment;
            this.Logger = logger;
        }

		public virtual IActionResult BadRequest(Exception ex)
		{
			string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
			this.Logger.LogError(msg);

			return BadRequest(msg);
		}




        public virtual async Task<IRestResponse> SendRequestAsync(string url, object param, string type)
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(url);
            switch (type)
            {
                case "GET":
                    request.Method = Method.GET;
                    request.RequestFormat = DataFormat.Json;
                    break;

                case "POST":
                    request.Method = Method.POST;
                    request.RequestFormat = DataFormat.Json;
                    break;

                case "PUT":
                    request.Method = Method.PUT;
                    request.RequestFormat = DataFormat.Json;
                    break;

                case "DELETE":
                    request.Method = Method.DELETE;
                    request.RequestFormat = DataFormat.Json;
                    break;

                case "PATCH":
                    request.Method = Method.PATCH;
                    request.RequestFormat = DataFormat.Json;
                    break;

                case "HEAD":
                    request.Method = Method.HEAD;
                    request.RequestFormat = DataFormat.Json;
                    break;

                default:
                    request.Method = Method.POST;
                    break;
            }

            string[] types = new string[] { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD" };

            request.AddHeader("Accept", "application/json");

            if (Array.IndexOf(types, type) > -1)
            {
                string token = HttpContext.Session.GetString(Constant.ACCESS_TOKEN); // Get Token from session
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddBody(JsonConvert.SerializeObject(param));
            }
            else
            {
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                AddParam(request, param);
            }

            IRestResponse response = await client.ExecuteAsync(request);

            return response;
        }

        private void AddParam(RestRequest request, object obj)
        {
            foreach (PropertyInfo key in obj.GetType().GetProperties())
            {
                request.AddParameter(key.Name, key.GetValue(obj));
            }
        }

        private string ParamToString(object obj)
        {
            List<string> lst = new List<string>();
            foreach (PropertyInfo key in obj.GetType().GetProperties())
            {
                lst.Add(string.Format("{0}={1}", key.Name, key.GetValue(obj)));
            }

            string str = string.Join("&", lst);

            return str;
        }

        public virtual async Task<IActionResult> ImportExcelAsync(IFormFile file)
        {
            if (file == null)
                return BadRequest(new { status = false, message = "File is null" });

            if (file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                return BadRequest(new { status = false, message = "Filetype incorrect" });

            try
            {
                string dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot", "Backup");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string filePath = Path.Combine(dir, file.FileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                string connectionString;
                switch (file.ContentType)
                {
                    case "application/vnd.ms-excel":
                        connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0; data source={filePath}; Extended Properties=Excel 8.0;";
                        break;

                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 12.0 Xml;HDR=YES;\"";
                        break;

                    default:
                        connectionString = "";
                        break;
                }

                DataTable table = new DataTable();
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();
                    DataTable tableSheet = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions: null);
                    if (tableSheet != null)
                    {
                        string sheetName = tableSheet.Rows[0]["TABLE_NAME"].ToString();

                        OleDbCommand command = connection.CreateCommand();
                        command.CommandText = $"SELECT * FROM [{sheetName}]";
                        command.CommandType = CommandType.Text;

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            table.Load(reader);
                        }
                    }
                }

                return Ok(new { status = true, message = JsonConvert.SerializeObject(table) });
            }
            catch (Exception exception)
            {
                string msg = exception.InnerException != null ? exception.InnerException.Message : exception.Message;

                return BadRequest(new { status = false, message = msg });
            }
        }

        public virtual Stream ExportToExcel(DataTable table, string fileName, string sheetName = "Sheet1")
        {
            Stream stream = new MemoryStream();

            string dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot", "templates");
            string filePath = Path.Combine(dir, fileName);
            FileInfo path = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(path))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].LoadFromDataTable(table, true, TableStyles.Medium6);
                int idx = 1;
                foreach (DataColumn column in table.Columns)
                {
                    if (column.DataType == typeof(DateTime))
                    {
                        worksheet.Column(idx).Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    if (column.DataType == typeof(TimeSpan))
                    {
                        worksheet.Column(idx).Style.Numberformat.Format = "HH:ss";
                    }
                    idx += 1;
                }

                package.SaveAs(stream);
                stream.Position = 0;
            }

            return stream;
        }
    }
}