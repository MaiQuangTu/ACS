var onCallAjax = function (url, method, dataType, contentType, data, async) {
    var setting = {
        url: url,
        method: method,
        cache: false,
        async: (async == null || async == true ? true : false),
        dataType: dataType,
        contentType: contentType,
        data: data,
        beforeSend: function (request) {
            request.setRequestHeader('Accept', 'application/' + dataType);
        }
    };

    return $.ajax(setting);
};

var onShowError = function (response) {
    var msg = '';

    switch (response.status) {
        //case 0:
        //    msg += '<p class="h5">' + 'Not connect.' + '</p>';
        //    msg += '<span>' + 'Verify Network.' + '</span>';
        //    break;
        case 200:
            return;
        case 401:
            msg += '<p class="h5">' + '401: Unauthorized.' + '</p>';
            msg += '<span>' + response.responseJSON + '</span>';
            break;
        case 404:
            msg += '<p class="h5">' + '404: Not found.' + '</p>';
            msg += '<span>' + response.responseJSON + '</span>';
            break;
        case 500:
            msg += '<p class="h5">' + '500: Internal Server Error.' + '</p>';
            msg += '<span>' + response.responseJSON + '</span>';
            break;

        default:
            msg += '<p class="h5">' + 'Other Error.' + '</p>';
            msg += '<span>' + response.responseJSON + '</span>';
            break;
    }


    bootbox.alert({
        size: 'small',
        message: msg,
        buttons: {
            ok: {
                label: 'Ok',
                className: 'dhl-btn-primary'
            }
        }
    });
};

var AjaxFuncTemp = {
    CallAjax: onCallAjax,
    ShowError: onShowError
};