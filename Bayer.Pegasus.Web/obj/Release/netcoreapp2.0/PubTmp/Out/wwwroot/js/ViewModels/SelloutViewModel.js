function SelloutViewModel(dashboard) {

    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "Sellout");

    self.Dashboard = dashboard;

    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);
    
    self.ShouldSave = ko.observable(false);

    self.Filter = ko.observable(true);

    self.ChartData = ko.observableArray([]);

    self.ReportDateViewModel = new ReportDateViewModel(dashboard);

    self.UpdateData = function (results) {
        self.ChartData.removeAll();

        ko.utils.arrayForEach(results, function (item) {

            self.ChartData.push(item);
        });
    }


    self.TypeDataChart = ko.observable("Value");

    self.TypeDataChartByQuantity = function() {
        self.TypeDataChart("Quantity");
        var chart = $("#chart-results").is(":visible"); 

        if(chart) {
            self.GetDataChart();
        }
    }

    self.TypeDataChartByValue = function() {
        self.TypeDataChart("Value");
        var chart = $("#chart-results").is(":visible"); 

        if(chart) {
            self.GetDataChart();
        }
    }

    self.IntervalDataChart = ko.observable("");

    self.GenerateChart = function () {
        
        self.Filter(true);

        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), self.GetDataChart);
        } else {
            self.GetDataChart();
        }
    }
    
    self.GetDataChart = function (dashboard, customModel) {

        util.ValidationResults.ClearErrors();

        var vmObj = ko.toJS(self);

        var selector;
        delete vmObj['HistoryViewModel'];

        if (dashboard && self.IntervalDataChart() != "Custom") {
            delete vmObj['ReportDateViewModel'];
        }

        if (customModel) {
            vmObj = ko.toJS(customModel);
        }

       

        var data = ko.toJSON(vmObj);

        if (self.Dashboard) {
            $('#modalSellout').modal('hide');
        }
        else {
            $("#filtro-consulta").hide();
            $("#chart-results").fadeIn();
        }

        if (self.Dashboard) {
            selector = "#contentSelloutChart";
        }
        else {
            selector = "#chart-results .resumo";
        }

        ajaxFlow.StartLoadingChart(selector);

        $(selector).find('.grafico').attr('data-animate', !dashboard);
     
        $.ajax({
            type: "POST",
            url: util.mapUrl("/Sellout/DataChart"),
            data: data,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    

                    if (!self.Dashboard) {
                        $("#chart-results").hide();
                        $("#filtro-consulta").fadeIn();
                    }
                    

                    util.ValidationResults.ShowErrors(results);
                   
                }
                else {
                    self.UpdateData(results);
                }                
                if (!self.Dashboard) {
                    //Filtro de Parceiros
                    var filtroaplicado_parc = "";
                    jQuery.each(self.Partners(), function (i, partners) {
                        if (partners.label) {
                            if (filtroaplicado_parc) {
                                filtroaplicado_parc = filtroaplicado_parc + ", " + partners.label;
                            } else {
                                filtroaplicado_parc = "Parceiro(s): " + partners.label;
                            }

                        }

                    });

                    //Filtro de Unidades
                    var filtroaplicado_units = "";
                    jQuery.each(self.Units(), function (i, units) {
                        if (units.label) {
                            if (filtroaplicado_units) {
                                filtroaplicado_units = filtroaplicado_units + ", " + units.label;
                            } else {
                                filtroaplicado_units = " Unidade(s):" + units.label;
                            }

                        }

                    });

                    var filtroaplicado = filtroaplicado_units;

                    switch (self.ReportDateViewModel.TypeDate()) {
                        case "y":
                            filtroaplicado = filtroaplicado + " Ano: " + self.ReportDateViewModel.YearDate();
                            break;
                        //Safra
                        case "c":
                            filtroaplicado = filtroaplicado + " Safra: " + self.ReportDateViewModel.CropDate();
                            break;
                        //Mês
                        case "m":
                            filtroaplicado = filtroaplicado + " Mês: " + self.ReportDateViewModel.MonthDate();
                            break;
                        //Semana
                        case "w":
                            filtroaplicado = filtroaplicado + " Semana: " + self.ReportDateViewModel.WeekDate();
                            break;
                        //Últimos
                        case "l":
                            filtroaplicado = filtroaplicado + " Últimos: " + self.ReportDateViewModel.LastDate();
                            dateFilter.EndDate = DateTime.Now;
                            switch (self.ReportDateViewModel.LastType()) {
                                case "y":
                                    filtroaplicado = filtroaplicado + " Últimos Anos: " + self.ReportDateViewModel.LastDate();
                                    break;
                                case "m":
                                    filtroaplicado = filtroaplicado + " Últimos Meses: " + self.ReportDateViewModel.LastDate();
                                    break;
                                case "d":
                                    filtroaplicado = filtroaplicado + " Últimos dias: " + self.ReportDateViewModel.LastDate();
                                    break;
                            }
                            break;
                        //Personalizado
                        case "custom":
                            filtroaplicado = filtroaplicado + " Data Inicio: " + self.ReportDateViewModel.StartDate();
                            filtroaplicado = filtroaplicado + " Data Fim: " + self.ReportDateViewModel.EndDate();
                            break;
                    }

                    $('#FiltroAplicadoParc').text(filtroaplicado_parc);
                    $('#FiltroAplicado').text(filtroaplicado);       
                }

                ajaxFlow.FinishLoadingChart(selector);
                util.barchartTooltip();
                util.createAxisLegend($(selector), self.TypeDataChart());
        });
        
    }
}