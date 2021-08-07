function SelloutEvolutionViewModel(dashboard) {

    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "SelloutEvolution");

    if (!dashboard)
        self.CFOPViewModel = new CFOPViewModel();

    self.Dashboard = dashboard;

    self.Filter = ko.observable(true);
    
    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);

    self.Clients = ko.observableArray([]);

    self.Brands = ko.observableArray([]);

    self.Products = ko.observableArray([]);

    self.Cities = ko.observableArray([]);

    self.ShouldSave = ko.observable(false);

    self.ReportDateViewModel = new ReportDateViewModel(dashboard);

    self.GroupBy = ko.observable("");

    self.GroupByBrand = function() {
        self.GroupBy("Brand");
    }

    self.RemoveGroupBy = function() {
        self.GroupBy("");
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
            delete vmObj['CFOPViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), self.GetDataChart);
        } else {
            self.GetDataChart();
        }
    }

    self.GetDataChart = function (dashboard) {
        
        util.ValidationResults.ClearErrors();

        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        delete vmObj['CFOPViewModel'];

        if(dashboard) {
            delete vmObj['ReportDateViewModel'];
        }


        var data = ko.toJSON(vmObj);

        if (self.Dashboard) {
            $('#modalSelloutEvolution').modal('hide');
        }
        else {
            $("#filtro-consulta").hide();
            $("#chart-results").fadeIn();
        }


        if (self.Dashboard) {
            ajaxFlow.StartLoadingChart("#sell-out .evolucao .wrap-grafico");
        }
        else {
            ajaxFlow.StartLoadingChart("#chart-results .panel-body");

        }        


        $.ajax({
            type: "POST",
            url: util.mapUrl("/SelloutEvolution/DataChart"),
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
                    createChartEvolutionSellOut('#sell-out .evolucao .grafico', results.kpis, results.ticks, !dashboard, results.tooltips, self.TypeDataChart());

                }
                
                if (self.Dashboard) {
                    ajaxFlow.FinishLoadingChart("#sell-out .evolucao .wrap-grafico");
                }
                else {                                        
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

                    //Filtro de Cidades
                    var filtroaplicado_cities = "";
                    jQuery.each(self.Cities(), function (i, cities) {
                        if (cities.label) {
                            if (filtroaplicado_cities) {
                                filtroaplicado_cities = filtroaplicado_cities + ", " + cities.label;
                            } else {
                                filtroaplicado_cities = " Cidades(s):" + cities.label;
                            }

                        }
                    });
                    var filtroaplicado = filtroaplicado_units + filtroaplicado_clients + filtroaplicado_brands + filtroaplicado_products + filtroaplicado_cities;

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
                            switch (self.ReportDateViewModel.LastType()) {
                                case "y":
                                    filtroaplicado = filtroaplicado + " Último(s) Ano(s): " + self.ReportDateViewModel.LastDate();
                                    break;
                                case "m":
                                    filtroaplicado = filtroaplicado + " Último(s) Mese(s): " + self.ReportDateViewModel.LastDate();
                                    break;
                                case "d":
                                    filtroaplicado = filtroaplicado + " Último(s) dia(s): " + self.ReportDateViewModel.LastDate();
                                    break;
                            }
                            break;
                        //Personalizado
                        case "custom":
                            filtroaplicado = filtroaplicado + " Data Inicio: " + self.ReportDateViewModel.StartDate();
                            filtroaplicado = filtroaplicado + " Data Fim: " + self.ReportDateViewModel.EndDate();                            
                            break;
                    }
      
                    if (self.GroupBy()) {
                        filtroaplicado = filtroaplicado + " Agrupamento: " + self.GroupBy() + "; ";
                    }
                    $('#FiltroAplicadoParc').text(filtroaplicado_parc);
                    $('#FiltroAplicado').text(filtroaplicado);                           
                }      
                ajaxFlow.FinishLoadingChart("#chart-results .panel-body");
            }
        });

    }

}