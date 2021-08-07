function SelloutStockViewModel(dashboard) {

    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "SelloutStock");

    if (!dashboard)
        self.CFOPViewModel = new CFOPViewModel();

    self.ReportDateViewModel = new ReportDateViewModel(dashboard);

    self.Dashboard = dashboard;

    self.Filter = ko.observable(true);

    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);

    self.Brands = ko.observableArray([]);

    self.Products = ko.observableArray([]); 

    self.Last12Months = ko.observable(true);

    self.Year = ko.observable(new Date().getFullYear());

    self.ShouldSave = ko.observable(false);

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
            vmObj.Year = '';
        }
        
        var data = ko.toJSON(vmObj);

        if (self.Dashboard) {
            $('#modalSelloutStock').modal('hide');
        }
        else {
            $("#filtro-consulta").hide();
            $("#chart-results").fadeIn();
        }

        if(self.Dashboard)
            ajaxFlow.StartLoadingChart("#sell-out-estoque .evolucao");
        else
            ajaxFlow.StartLoadingChart("#chart-results .panel-body");
        
        $.ajax({
            type: "POST",
            url: util.mapUrl("/SelloutStock/DataChart"),
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
                    createChartSellOutStock('#sell-out-estoque .evolucao .grafico', results.kpis, results.ticks, !dashboard, results.tooltips, (typeof self.TypeDataChart != 'undefined') ? self.TypeDataChart() : 'Quantity');

                }
                                
                if (self.Dashboard)
                    ajaxFlow.FinishLoadingChart("#sell-out-estoque .evolucao");
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

                    if (self.Year()) {
                        filtroaplicado = filtroaplicado + "Ano Comparativo: " + self.Year();
                    }
                    
                    $('#FiltroAplicadoParc').text(filtroaplicado_parc);
                    $('#FiltroAplicado').text(filtroaplicado);       

                    ajaxFlow.FinishLoadingChart("#chart-results .panel-body");
                }
                    
                        
               
            }
        });
    }

}