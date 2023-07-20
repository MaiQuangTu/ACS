var onConvertToLocalDate = function (date) {
    if (date == null)
        return null;

    var datetime = new Date(date);

    var dateValue = datetime.setTime(datetime.getTime() - datetime.getTimezoneOffset() * 60 * 1000);

    return new Date(dateValue);
};

var onConvertToDateTimeStr = function (date) {
    if (date == null)
        return null;

    var dateStr = new Date(date).toLocaleDateString('en-GB');
    var timeStr = new Date(date).toLocaleTimeString();
    return dateStr + " " + timeStr;
};

var onStringToDate = function (dateStr) {
    var date = new Date(dateStr + 'Z');
    if (date.toString() == 'Invalid Date')
        return '';

    return date.toLocaleDateString('en-GB');
};

var onViewPDF = function (name, value) {
    var pdfWindow = window.open();
    pdfWindow.document.write('<iframe width="100%" height="100%" src="data:application/pdf;base64, ' + value + '" frameborder="0" allowfullscreen></iframe>');
    pdfWindow.document.title = name;
};

var onBlock = function () {
    let html = '';
    html += '<div class="loading-spinner">';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '<div></div>';
    html += '</div>';
    
    $.blockUI({
        css: {
            backgroundColor: 'transparent',
            border: 'none'
        },
        message: html,
        baseZ: 1500,
        overlayCSS: {
            backgroundColor: '#000',
            opacity: 0.5,
            cursor: 'wait'
        }
    });
};

var onUnBlock = function () {
    $.unblockUI();
};

var CommonFunc = {
    ConvertToLocalDate: onConvertToLocalDate,
    StringToDate: onStringToDate,
    ViewPDF: onViewPDF,
    Block: onBlock,
    UnBlock: onUnBlock,
    ConvertToDateTimeStr: onConvertToDateTimeStr,
}