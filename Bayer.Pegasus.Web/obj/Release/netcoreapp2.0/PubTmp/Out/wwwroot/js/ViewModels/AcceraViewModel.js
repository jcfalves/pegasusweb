function AcceraViewModel() {

    var self = this;


    self.Partners = ko.observableArray([]);

    self.HistoryViewModel = new HistoryViewModel(this, "Accera");

    self.ShouldSave = ko.observable(false);

    self.Execute = function () {
        util.ValidationResults.ClearErrors();

        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), self.GenerateResults);
        } else {
            self.GenerateResults();
        }
    }

    self.GenerateResults = function () {
        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        var data = ko.toJSON(vmObj);
        
        $("#filtro-consulta").hide();
        $("#report-results table").hide();

        $("#report-results").fadeIn();

        ajaxFlow.StartLoadingChart("#report-results");   

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Accera/Results"),
            data: data,
            contentType: "application/json",
            dataType: "html",
            success: function (results) {

                

                if (results.hasErrors) {
                    $("#filtro-consulta").fadeIn();
                    $("#report-results").hide();

                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    $("#report-results").html(results);

                    $("#report-results table").show();


                    $('#report-results h2.title-tab').click(function () {
                        $('#tabs-panel .tab').removeClass('active');
                        $('.panel-filtros').hide();
                        $('#tabs-panel .tab[data-open="filtro-consulta"').addClass('active');
                        $('#' + $(this).attr('data-open')).fadeIn();
                    });

                    $('.print-tab').click(function () {
                        window.print();
                    });

                    var rows = $('#report-results table tr').length;

                    if (rows > 2) {
                        $('.ReportAccera:first').width($('.ReportAccera:last').width());
                        $('.ReportAccera:first th[rowspan="2"]').each(function () {
                            $(this).width($('.ReportAccera:last tr:first td').eq($(this).index()).width());
                            if (parseInt($(this).css('min-width')) > $(this).width())
                                $(this).width(parseInt($(this).css('min-width')) - ($(this).outerWidth() - $(this).width()));
                        });
                    }
                    
                    $(window).scroll(function () {
                        if ($(window).scrollTop() - $('#AcceraBody').offset().top > 0 - $('#AcceraHeader').height())
                            $('#AcceraHeader').css({ 'position': 'fixed', 'width': 'calc(100% - 40px)', 'overflow': 'hidden' });
                        else {
                            $('#AcceraHeader').css({ 'position': 'initial', 'width': '100%', 'overflow': 'initial' });
                            $("#AcceraHeader table.table-fixed").css('transform', 'translateX(0)');
                        }
                    })

                    ajaxFlow.FinishLoadingChart("#report-results .formContent");

                    new SimpleBar($('.table-wrapper')[0], {
                        autoHide: false
                    });

                    $(".simplebar-content").scroll(function () {
                        if ($('#AcceraHeader').css('position') == 'fixed')
                            $("#AcceraHeader table.table-fixed").css('transform', 'translateX(' + (this.scrollLeft * -1) + 'px)');
                        else
                            $("#AcceraHeader table.table-fixed").css('transform', 'translateX(0)');
                    });
                }
                
            }
        });

    }

}