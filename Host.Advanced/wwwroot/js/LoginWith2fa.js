function switchToAuthenticator() {
    $('#promptModal').modal('hide');
    $('#u2fLogin').toggleClass('hidden');
    $('#authenticatorLogin').toggleClass('hidden');
    $('#method').val('AuthenticatorCode');
}

var errorMap = {
    1: 'Unknown error, try again',
    2: "Bad request error, try again",
    3: "This key isn't supported, please try another one",
    4: 'The device is already registered, please login',
    5: 'Authentication timed out. Please reload to try again.'
};
$('#switchButton').on('click', switchToAuthenticator);

$(function(){
    if ($('#method').val() !== 'AuthenticatorCode') {
        $('#promptModal').modal('show');
        u2f.sign(
            $('#AppId').val(),
            $('#Challenge').val(),
            JSON.parse($('#Challenges').val()),
            function(data) {
                console.log("call back data: ", data);

                if (data.errorCode) {
                    $('#promptModal').modal('hide');
                    console.log(errorMap[data.errorCode]);
                } else {
                    $('#promptModal').modal('hide');
                    $('#DeviceResponse').val(JSON.stringify(data));
                    $('#loginForm').submit();
                }

                return "";
            }, 600);
    }
});

