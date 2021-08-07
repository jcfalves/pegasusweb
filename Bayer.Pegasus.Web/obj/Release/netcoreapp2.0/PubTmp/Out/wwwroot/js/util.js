var util = new function () {
    var self = this;

    self.labelForAxis = {
        Quantity: "Quantidade/L",
        Value: "Valor (R$)"
    }


    self.mapUrl = function (url) {
        if (window.location.host.indexOf("localhost") > -1)
            return url;
        return '/pegasus' + url;
    }

    self.ShowSuccess = function (message, notRedirect) {

        document.querySelector("#modalWindow .modal-body").innerHTML = message;
        $('#modalWindow').modal('toggle');

        $('#modalWindow').on('hidden.bs.modal', function () {
            if (!notRedirect)
                document.location.href = "../";
        })
    }

    self.ShowSuccessMessage = function (message) {

        document.querySelector("#modalWindowSucess .modal-body").innerHTML = message;
        $('#modalWindowSucess').modal('toggle');
    }

    self.ValidationResults = new ValidationResults();

    self.mapData = function (data, viewModel) {

        try {
            var map = ko.mapping.fromJS(data, {}, viewModel);
            ko.applyBindings(map);

        }
        catch (e) {
            alert(e);
        }
    }

    self.adjustHeightContent = function () {
        $(window.parent.document).find('#content-pegasus').parent().css({ 'padding-top': $(window.parent.document).find('#menu').height(), 'height': 'calc(100% - ' + ($(window.parent.document).find('#menu').height() - 5) + 'px)'  });
    }

    self.barchartTooltip = function () {
        $('.resumo .grafico > .item .barra .progress-bar').hover(
            function () {
                $('.flotTip').remove();
                $('body').append('<div class="flotTip">' + $(this).attr('data-tip') + '</div>');
                $(document).on('mousemove', function (e) {
                    $('.flotTip').css({
                        left: e.pageX + 5,
                        top: e.pageY + 5,
                        position: 'absolute'
                    });
                });
            },
            function () {
                $('.flotTip').remove();
            }
        );
    }

    self.ShowConfirm = function (message, onConfirm) {
        
        toastr.options = {
            "closeButton": true,
            "debug": true,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center confirmToast",
            "preventDuplicates": false,
            "onclick": null,
            "showEasing": "swing",
            "showMethod": "fadeIn",
            "timeOut": 3000
        }
        
        toastr.remove();
        
        toastr["info"]('<div><p class="text-center">' + message + '</p></div><div class="text-center"><button type="button" id="btn-toastr-yes" class="btn btn-toastr-yes">Sim</button><button type="button" id="btn-toastr-no" class="btn btn-toastr-no" style="margin: 0 8px 0 8px">Não</button></div>');

        $("#btn-toastr-yes").click(function () {
            onConfirm();
        });

        $("#btn-toastr-no").click(function () { });
        
    }

    self.ShowWarning = function (message) {

        toastr.options = {
            "closeButton": true,
            "debug": true,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center confirmToast",
            "preventDuplicates": false,
            "onclick": null,
            "showEasing": "swing",
            "showMethod": "fadeIn",
            "timeOut": 3000
        }

        toastr.remove();

        toastr["warning"](message)
    }

    self.createAxisLegend = function (selector, typeDataChart) {
        $(selector).find('.axisLabel').remove();
        $(selector).append('<div class="axisLabel">' + self.labelForAxis[typeDataChart] + '</div>');
    }

    self.adjustBarWidth = function(selector) {
        $(selector).find('.barra').css('width', 'calc(100% - ' + ($(selector).find('.valor:first').outerWidth() + 10) + 'px)');
    }
}


$.extend($.datepicker, { // Extends datepicker with shortcuts.
    customKeyUp: function (event) {

        var value = event.target.value;
        
        if (value.length == 10) {
            console.log(value);
            var picker = $(event.target).data("DateTimePicker");

            picker.date(value);

            event.stopPropagation();
            event.preventDefault();


        }
        
    }
});


ko.bindingHandlers.bootstrapSelect = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $('select').selectpicker();

        $('select').on('loaded.bs.select', function (e) {
            new SimpleBar($('.dropdown-menu.inner')[0], {
                autoHide: false
            })
        });

        $(element).selectpicker('val', ko.unwrap(valueAccessor()));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $(element).selectpicker('val', ko.unwrap(valueAccessor()));
    }
}

ko.bindingHandlers.datePicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //initialize datepicker with some optional options
        var format = (typeof allBindingsAccessor().datePickerFormat != "undefined") ? allBindingsAccessor().datePickerFormat : 'DD/MM/YYYY';
        var viewMode = (typeof allBindingsAccessor().datePickerViewMode != "undefined") ? allBindingsAccessor().datePickerViewMode : 'days';

        var options = allBindingsAccessor().dateTimePickerOptions || {
            useCurrent: false,
            dayViewHeaderFormat: "D [de] MMMM, YYYY",
            viewMode: viewMode,
            format: format,
            locale: 'pt-br',
            widgetPositioning: { vertical: 'auto' }
        };

        
        $(element).datetimepicker(options);
        
        $(element).keyup(function (event) {
            $.datepicker.customKeyUp(event);
        });
        
        var currentDate = ko.unwrap(valueAccessor());


        var picker = $(element).data("DateTimePicker");

        if (typeof currentDate != "string")
            currentDate = currentDate.toString();

        picker.date(currentDate);
        
        //when a user changes the date, update the view model
        ko.utils.registerEventHandler(element, "dp.change", function (event) {
            var value = valueAccessor();
            
            if (typeof event.date.format != "undefined") {
                if (options.format == "MMMM") {
                    options.format = "M";
                }
                var date = event.date.format(options.format);
                if (ko.isObservable(value)) {
                    value(date);
                }
            } else {
                value("");
            }

            var valueUnwrapped = ko.unwrap(value);
            
            var el = event.target || event.srcElement;
            el = $(el).parent();

            $(el).removeClass('validationError');
            $(el).find('.validationMessage').remove();
        });

        ko.utils.registerEventHandler(element, "dp.update dp.show", function (event) {
            if ($(".bootstrap-datetimepicker-widget .datepicker-days table tr:last td.new").length == 7) {
                $(".bootstrap-datetimepicker-widget .datepicker-days table tr:last").hide();
            }
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            var picker = $(element).data("DateTimePicker");
            if (picker) {
                picker.destroy();
            }
        });

        $('.form-group.fa-calendar').on('focus click', function () {
            $(this).find('.datepicker').data('DateTimePicker').show();
        });
    },
    update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

        var picker = $(element).data("DateTimePicker");
        //when the view model is updated, update the widget
        if (picker) {
            var koDate = ko.utils.unwrapObservable(valueAccessor());
            var currentDate = ko.unwrap(valueAccessor());

            if (typeof koDate != "string")
                koDate = koDate.toString();

            picker.date(koDate);

        }
    }
};

// Here's a custom Knockout binding that makes elements shown/hidden via jQuery's fadeIn()/fadeOut() methods
// Could be stored in a separate utility library
ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();


        $(element).toggle(ko.unwrap(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slowly fade the element in or out
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).fadeIn() : $(element).hide();
    }
};

