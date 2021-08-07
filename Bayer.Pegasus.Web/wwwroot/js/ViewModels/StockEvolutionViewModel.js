function StockEvolutionViewModel(dashboard) {

    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "StockEvolution");

    self.Dashboard = dashboard;

    self.Filter = ko.observable(true);

    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);

    self.Brands = ko.observableArray([]);

    self.Products = ko.observableArray([]);
    
    self.ShouldSave = ko.observable(false);

    self.ReportDateViewModel = new ReportDateViewModel(dashboard);

    self.GroupBy = ko.observable("");

    self.GroupByBrand = function() {
        self.GroupBy("Brand");
    }

    self.RemoveGroupBy = function() {
        self.GroupBy("");
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

    self.GetDataChart = function (dashboard) {

        util.ValidationResults.ClearErrors();

        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];

        if(dashboard) {
            delete vmObj['ReportDateViewModel'];
        }

        var data = ko.toJSON(vmObj);

        if (self.Dashboard) {
            $('#modalStockEvolution').modal('hide');
        }
        else {
            $("#filtro-consulta").hide();
            $("#chart-results").fadeIn();
        }

        if (self.Dashboard) {
            ajaxFlow.StartLoadingChart("#estoque .evolucao .wrap-grafico");
        }
        else {
            ajaxFlow.StartLoadingChart("#chart-results .panel-body");
        
        }

        $.ajax({
            type: "POST",
            url: util.mapUrl("/StockEvolution/DataChart"),
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
                    createChartEvolutionStock('#estoque .evolucao .grafico', results.kpis, results.ticks, !dashboard, results.tooltips, (typeof self.TypeDataChart != 'undefined') ? self.TypeDataChart() : 'Quantity');

                }




                if (self.Dashboard) {
                    ajaxFlow.FinishLoadingChart("#estoque .evolucao .wrap-grafico");
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

                    ajaxFlow.FinishLoadingChart("#chart-results .panel-body");
                }

            }
        });

    }

}