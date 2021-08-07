function ValidationResults() {

    var self = this;

    self.Errors = new Array();

    self.Fields = new Array();

    self.ShowErrors = function (dados) {

        toastr.options = {
            "closeButton": true,
            "debug": true,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center errorToast",
            "preventDuplicates": false,
            "onclick": null,
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut",
            "timeOut": 5000
        }

        if (dados.success) {
            return false;
        }

        self.Errors = new Array();


        if (dados.errors)
            self.Errors = dados.errors;

        if (dados.fields) {
            self.Fields = dados.fields;
        }

        self.Fields.forEach(function (el) {
            $('#' + el).parent().find('.validationMessage').remove();
            $('#' + el).parent().addClass('validationError').append('<span class="validationMessage"><i class="fa fa-times-circle" aria-hidden="true"></i>Erro na validação do campo</span>');;
        });

        $('.validationError input').keyup(function (e) {
            console.log($(this));
            if(e.keyCode !== 13)
                self.ClearErrors($(this));
        });

        var errors = self.Errors.join('<br>');

        toastr.remove();
        toastr["error"](errors);

        return true;

    }

    
    self.ClearErrors = function (el) {
        if (typeof el == "undefined") {
            el = $('.validationError');
        } else {
            el = $(el).parents('.validationError')
        }
        $(el).removeClass('validationError');
        $(el).find('.validationMessage').fadeOut(1000, function () {
            $(el).find('.validationMessage').remove();
        });
        toastr.clear();
    }

}