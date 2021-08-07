function DashboardViewModel() {

    var self = this;

    self.Partners = ko.observableArray([]);

    self.Units = ko.observableArray([]);

    self.CFOPViewModel = new CFOPViewModel();

    self.ReportDateViewModel = new ReportDateViewModel(true);

    self.TypeDataChart = ko.observable("Value");

    self.TypeDataChartByQuantity = function() {
        self.TypeDataChart("Quantity");
        self.SetTypeDataChart("Quantity");
        self.GenerateDashboard(true);
    }

    self.TypeDataChartByValue = function() {
        self.TypeDataChart("Value");
        self.SetTypeDataChart("Value");
        self.GenerateDashboard(true);
    }
    

    self.IntervalDataChart = ko.observable("Last12Months");

    self.IntervalDataChartByYear = function() {
        self.IntervalDataChart("Year");
        self.GenerateDashboard(true);
    }


    self.IntervalDataChartByLast12Months = function () {
        self.IntervalDataChart("Last12Months");
        self.GenerateDashboard(true);
    }

    self.IntervalDataChartByMonth = function() {
        self.IntervalDataChart("Month");
        self.GenerateDashboard(true);
    }

    self.IntervalDataChartByWeek = function() {
        self.IntervalDataChart("Week");
        self.GenerateDashboard(true);
    }

    self.IntervalDataChartCustom = function() {
        
        self.IntervalDataChart("Custom");
        
        openModal("#modalDashboard");
    };

    self.GenerateChartDashboard = function () {

        util.ValidationResults.ClearErrors();

        var vmObj = ko.toJS(self);
        delete vmObj['TopProductsViewModel'];
        delete vmObj['SelloutEvolutionViewModel'];       
        delete vmObj['StockEvolutionViewModel'];   
        delete vmObj['TopStockProductsViewModel'];   
        delete vmObj['SelloutStockViewModel'];
        delete vmObj['CFOPViewModel'];
        

        var data = ko.toJSON(vmObj);

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Dashboard/ValidateFilter"),
            data: data,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    $('#modalDashboard').modal('hide');
                    self.GenerateDashboard(true);
                }

            }
        });

        
    }

    self.TopProductsViewModel = new TopProductsViewModel(true);

    self.StockEvolutionViewModel = new StockEvolutionViewModel(true);

    self.SelloutEvolutionViewModel = new SelloutEvolutionViewModel(true);

    self.TopStockProductsViewModel = new TopStockProductsViewModel(true);

    self.SelloutStockViewModel = new SelloutStockViewModel(true);


    self.GenerateDashboardAsync = function (noFilter) {
        
        var flag = true;        
        var vmObj = ko.toJS(self);
        delete vmObj['TopProductsViewModel'];
        delete vmObj['SelloutEvolutionViewModel'];       
        delete vmObj['StockEvolutionViewModel'];   
        delete vmObj['TopStockProductsViewModel'];   
        delete vmObj['SelloutStockViewModel'];
        delete vmObj['CFOPViewModel']; 
        
        if(self.IntervalDataChart() != "Custom") {
            delete vmObj['ReportDateViewModel'];    
        }
        else {
            flag = false;
        }

      

        self.SetIntervalDataChart(self.IntervalDataChart());

        
        var mappingOptions = {
            'ignore': ["__ko_mapping__"]
        };
        
        ko.mapping.fromJS(vmObj, mappingOptions, self.TopProductsViewModel);
        ko.mapping.fromJS(vmObj, mappingOptions, self.SelloutEvolutionViewModel);
        ko.mapping.fromJS(vmObj, mappingOptions, self.StockEvolutionViewModel);
        ko.mapping.fromJS(vmObj, mappingOptions, self.TopStockProductsViewModel);
        ko.mapping.fromJS(vmObj, mappingOptions, self.SelloutStockViewModel);

        self.SelloutEvolutionViewModel.Filter(!noFilter);
        self.TopProductsViewModel.Filter(!noFilter);
        self.StockEvolutionViewModel.Filter(!noFilter);
        self.TopStockProductsViewModel.Filter(!noFilter);
        self.SelloutStockViewModel.Filter(!noFilter);


        self.SelloutEvolutionViewModel.GetDataChart(flag);
        self.TopProductsViewModel.GetDataChart(flag);
        self.StockEvolutionViewModel.GetDataChart(flag);
        self.TopStockProductsViewModel.GetDataChart(flag);
        self.SelloutStockViewModel.GetDataChart(flag);
        
    }

    self.GenerateDashboardSync = function (noFilter) {
        var data = JSON.stringify({ IntervalDataChart: self.IntervalDataChart(), TypeDataChart: self.TypeDataChart() })


        ajaxFlow.StartLoadingChart("#contentSelloutChart");
        ajaxFlow.StartLoadingChart("#estoque .evolucao .wrap-grafico");
        ajaxFlow.StartLoadingChart("#sell-out .evolucao .wrap-grafico");
        ajaxFlow.StartLoadingChart("#contentTopStockProductsChart");
        ajaxFlow.StartLoadingChart("#sell-out-estoque .evolucao");

        $.ajax({
            type: "POST",
            url: util.mapUrl("/Dashboard/DataChart"),
            contentType: "application/json",
            data: data,
            success: function (dashboardData) {
                createChartEvolutionSellOut('#sell-out .evolucao .grafico', dashboardData.selloutEvolution, dashboardData.ticks, false);
                createChartEvolutionStock('#estoque .evolucao .grafico', dashboardData.stockEvolution, dashboardData.ticks, false);
                createChartSellOutStock('#sell-out-estoque .evolucao .grafico', dashboardData.selloutStock, dashboardData.ticks, false);
                self.TopProductsViewModel.UpdateData(dashboardData.sellout);
                self.TopStockProductsViewModel.UpdateData(dashboardData.topStockProducts);

                self.SetIntervalDataChart('');

                ajaxFlow.FinishLoadingChart("#contentSelloutChart");
                ajaxFlow.FinishLoadingChart("#estoque .evolucao .wrap-grafico");
                ajaxFlow.FinishLoadingChart("#sell-out .evolucao .wrap-grafico");
                ajaxFlow.FinishLoadingChart("#contentTopStockProductsChart");
                ajaxFlow.FinishLoadingChart("#sell-out-estoque .evolucao");
            }
        });

    }

    self.GenerateDashboard = function (noFilter) {
        self.GenerateDashboardAsync(noFilter);

        $('input[type="checkbox"], input[type="radio"]').each(function () {
            var indexId = $(this).parents('.modal').index();
            $(this).attr('id', $(this).attr('id') + indexId);
            $(this).next('label').attr('for', $(this).next('label').attr('for') + indexId);
        });
    }

    self.SetIntervalDataChart = function (value) {
        self.TopProductsViewModel.IntervalDataChart(value);
        self.StockEvolutionViewModel.IntervalDataChart(value);
        self.SelloutEvolutionViewModel.IntervalDataChart(value);
        self.TopStockProductsViewModel.IntervalDataChart(value);
        self.SelloutStockViewModel.IntervalDataChart(value);
    }

    self.SetTypeDataChart = function (value) {
        self.TopProductsViewModel.TypeDataChart(value);
        self.SelloutEvolutionViewModel.TypeDataChart(value);
    }

}