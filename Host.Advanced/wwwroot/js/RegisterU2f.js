var errorMap = {
    1: 'Unknown error, try again',
    2: "Bad request error, try again",
    3: "This key isn't supported, please try another one",
    4: 'The device is already registered, please login',
    5: 'Authentication timed out. Please reload to try again.'
};

$(function() {
    $('#promptModal').modal('show');
    var request = { "challenge": $('#Challenge').val(), "version": $('#Version').val(), "appId": $('#AppId').val() };
    var registerRequests = [{version: request.version, challenge: request.challenge, appId: request.appId}];
    console.log(request);
    $('#promptModal').modal('show');
    u2f.register(request.appId, registerRequests, [],
        function(data) {
            if (data.errorCode) {
                $('#promptModal').modal('hide');
                console.log(errorMap[data.errorCode]);
                return false;
            }
            console.log("Register callback", data);
            $('#promptModal').modal('hide');
            $('#DeviceName').val($('#modalDeviceName').val());
            $('#DeviceResponse').val(JSON.stringify(data));
            $('#registerDevice').submit();

            return false;
        }, 600);
});
