function TopClientsViewModel() {
    
    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "TopClients");

    self.CFOPViewModel = new CFOPViewModel();
    
    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);

    self.Brands = ko.observableArray([]);

    self.Products = ko.observableArray([]);

    self.NumberClients = ko.observable("5");

    self.Year = ko.observable(new Date().getFullYear());

    self.ShouldSave = ko.observable(false);


    self.TypeDataChart = ko.observable("Value");

    self.TypeDataChartByQuantity = function () {
        self.TypeDataChart("Quantity");

        var chart = $("#chart-results").is(":visible");

        if (chart) {
            self.GetDataChart();
        }

    }

    self.TypeDataChartByValue = function () {
        self.TypeDataChart("Value");
        var chart = $("#chart-results").is(":visible");

        if (chart) {
            self.GetDataChart();
        }
    }

    self.ChartData = ko.observableArray([]);

    self.GenerateChart = function () {
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
        var data = ko.toJSON(vmObj);

        $("#filtro-consulta").hide();
        $("#chart-results").fadeIn();
        
        ajaxFlow.StartLoadingChart("#chart-results .resumo");

        $.ajax({
            type: "POST",
            url: util.mapUrl("/TopClients/DataChart"),
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


                    //Filtro de Marcas
                    var filtroaplicado_brands = "";
                    jQuery.each(self.Brands(), function (i, brands) {
                        if (brands.label) {
                            if (filtroaplicado_brands) {
                                filtroaplicado_brands = filtroaplicado_brands + ", " + brands.label;
                            } else {
                                filtroaplicado_brands = " Marca(s):" + brands.label;
                            }

                        }
                    });

                    //Filtro de Produtos
                    var filtroaplicado_products = "";
                    jQuery.each(self.Products(), function (i, products) {
                        if (products.label) {
                            if (filtroaplicado_products) {
                                filtroaplicado_products = filtroaplicado_products + ", " + products.label;
                            } else {
                                filtroaplicado_products = " Produtos(s):" + products.label;
                            }

                        }
                    });

                    var filtroaplicado = filtroaplicado_units + filtroaplicado_brands + filtroaplicado_products;
                    if (self.NumberClients()) {
                        filtroaplicado = filtroaplicado + " Numeros de Cliente(s):" + self.NumberClients() + "; ";
                    }
                    if (self.Year()) {
                        filtroaplicado = filtroaplicado + " Ano: " + self.Year() + "; ";
                    }

                    $('#FiltroAplicadoParc').text(filtroaplicado_parc);
                    $('#FiltroAplicado').text(filtroaplicado);                    
                }
                
                util.adjustBarWidth('#chart-results');
                util.barchartTooltip();
                util.createAxisLegend($("#chart-results .resumo"), (typeof self.TypeDataChart != 'undefined') ? self.TypeDataChart() : 'Value');
                ajaxFlow.FinishLoadingChart("#chart-results .resumo");
            }
        });



    }
}