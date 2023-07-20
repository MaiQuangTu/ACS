
namespace ACS.API.Services
{
	using DAL.DataAccess.Models;
	using ACS.API.Common;
	using System.Text;
	using LIB.Repositories;
	using LIB.Infrastructure;
	using System;

	public interface IShortenerURLService
	{
		ShortenerURL FindShortUrlByCodition(string? urlStr);
		ShortenerURL FindLongUrlByCodition(string? urlStr);

		Task<ShortenerURL> Add(string? urlStr);


		public class ShortenerURLService : IShortenerURLService
		{
			private readonly IShortenerURLRepository _repository;

			private readonly IUnitOfWork _unitOfWork;
			private readonly IConfiguration _configuration;

			public ShortenerURLService(IShortenerURLRepository repository, IUnitOfWork unitOfWork)
			{
				this._repository = repository;
				this._unitOfWork = unitOfWork;
			}

			public async Task<ShortenerURL> Add(string urlStr)
			{
				ShortenerURL model = new ShortenerURL();
				try
				{
					int count = this._repository.Get().Count();
					string strText = count.ToString("000000");
					string shortUrl = RecursionCheckShortUrl(GenerateShortURL());
					ShortenerURL data = new ShortenerURL();
					data.LongUrl = urlStr;
					data.ShortUrl = shortUrl;

					model = this._repository.Add(data);
					this._unitOfWork.Commit();
					return model;
				}
				catch (Exception ex)
				{
					Console.Write(ex);
					return model;
				}
			}

			public string RecursionCheckShortUrl(string shortUrl)
			{
				int count = this._repository.GetByCodition(x => x.ShortUrl == shortUrl).Count();
				if(count == 0)
				{
					return shortUrl;
				}
				else
				{
					return RecursionCheckShortUrl(GenerateShortURL());
				}
			}

			public static string GenerateShortURL()
			{
				string[] alphabet = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

				string shortUrl="";
				for (int i = 0; i <= 5; i++)
				{
					shortUrl += alphabet[new Random().Next(0, alphabet.Length)];
				}
				return shortUrl;
			}

			public ShortenerURL FindShortUrlByCodition(string? urlStr)
			{
				return this._repository.Get().FirstOrDefault(x => x.ShortUrl == urlStr);
			}

			public ShortenerURL FindLongUrlByCodition(string urlStr)
			{
				return this._repository.Get().FirstOrDefault(x => x.LongUrl == urlStr);
			}
		}
	}
}





















































