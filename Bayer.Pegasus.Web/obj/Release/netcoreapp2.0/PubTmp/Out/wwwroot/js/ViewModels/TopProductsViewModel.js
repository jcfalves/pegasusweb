function TopProductsViewModel(dashboard) {

    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "TopProducts");

    if(!dashboard)
        self.CFOPViewModel = new CFOPViewModel();

    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);

    self.Clients = ko.observableArray([]);
    
    self.NumberProducts = ko.observable("5");

    self.Year = ko.observable(new Date().getFullYear());

    self.ShouldSave = ko.observable(false);

    self.TypeDataChart = ko.observable("Value");

    self.IntervalDataChart = ko.observable("");
    
    self.GroupBy = ko.observable("");

    self.GroupByBrand = function () {
        self.GroupBy("Brand");
    }

    self.RemoveGroupBy = function () {
        self.GroupBy("");
    }

    self.Filter = ko.observable(true);

    self.Dashboard = dashboard;

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

    self.ChartData = ko.observableArray([]);

    self.GenerateChart = function () {
        
        self.Filter(true);
    
        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            delete vmObj['ChartData'];
            delete vmObj['CFOPViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), self.GetDataChart);
        } else {
            self.GetDataChart();
        }
    }

    self.UpdateData = function (results) {

        self.ChartData.removeAll();

        ko.utils.arrayForEach(results, function (item) {

            self.ChartData.push(item);
        });

    }

    self.GetDataChart = function () {
        util.ValidationResults.ClearErrors();
    
        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        delete vmObj['ChartData'];
        delete vmObj['CFOPViewModel'];


        if (!self.Filter()) {
            vmObj.Year = '';
        }

        var data = ko.toJSON(vmObj);

        if (self.Dashboard) {
            $('#modalTopProducts').modal('hide');
        }
        else {
            $("#filtro-consulta").hide();
            $("#chart-results").fadeIn();
        }


        if (self.Dashboard) {
            selector = "#contentTopProductsChart";
        }
        else {
            selector = "#chart-results .resumo";
        }

        ajaxFlow.StartLoadingChart(selector);   

        $(selector).find('.grafico').attr('data-animate', !dashboard);

        $.ajax({
            type: "POST",
            url: util.mapUrl("/TopProducts/DataChart"),
            data: data,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);

                    if (!self.Dashboard) {
                        $("#chart-results").hide();
                        $("#filtro-consulta").fadeIn();
                    }

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

                    //Filtro de Clientes
                    var filtroaplicado_clients = "";
                    jQuery.each(self.Clients(), function (i, clients) {
                        if (clients.label) {
                            if (filtroaplicado_clients) {
                                filtroaplicado_clients = filtroaplicado_clients + ", " + clients.label;
                            } else {
                                filtroaplicado_clients = " Cliente(s):" + clients.label;
                            }

                        }
                    });

                    var filtroaplicado = filtroaplicado_units + filtroaplicado_clients;
                    if (self.NumberProducts()) {
                        filtroaplicado = filtroaplicado + " Numeros de Produtos:" + self.NumberProducts() + "; ";
                    }
                    if (self.Year()) {
                        filtroaplicado = filtroaplicado + " Ano: " + self.Year() + "; ";
                    }
                    if (self.GroupBy()) {
                        filtroaplicado = filtroaplicado + " Agrupamento: " + self.GroupBy() + "; ";
                    }
                    $('#FiltroAplicadoParc').text(filtroaplicado_parc);
                    $('#FiltroAplicado').text(filtroaplicado);
                }                
                util.adjustBarWidth(selector);
                util.barchartTooltip();
                util.createAxisLegend($("#chart-results .resumo"), self.TypeDataChart());
                ajaxFlow.FinishLoadingChart(selector);
            }
        });



    }

}