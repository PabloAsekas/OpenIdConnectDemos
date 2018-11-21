$("[data-js='remove-device']").on('click',
    function (event) {
        event.preventDefault();
        var identifier = $(this).data('id');
        var form = $(this).closest('form');
        $('#deviceId').val(identifier);
        form.submit();
    });