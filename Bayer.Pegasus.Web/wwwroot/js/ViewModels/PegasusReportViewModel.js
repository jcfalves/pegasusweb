function PegasusReportViewModel() {

    var self = this;

    self.HistoryViewModel = new HistoryViewModel(this, "PegasusReport");

    self.ShouldSave = ko.observable(false);

    self.EmailTo = ko.observable("");

    self.SaveHistory = function (callback) {
        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), callback);
        } else {
            if (typeof callback != "undefined")
                callback();
        }
    }
    
    self.CreatePivot = function () {

        util.ValidationResults.ClearErrors();
        
        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), self.OnGenerateExcel);
        } else {
            self.OnCreatePivot();
        }

    }

    self.OnCreatePivot = function () {
        

        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        var data = ko.toJSON(vmObj);

        $("#ModalPivot").modal("hide");

        $("#LoadingExcel").show();
        $("#LoadedExcel").hide();

        $('#ModalExcel').modal('show');

        $.ajax({
            type: "POST",
            url: util.mapUrl("/PegasusReport/GeneratePivot"),
            data: data,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {

                    $('#ModalExcel').modal('hide');

                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    var identifier = results.Identifier;


                    var url = util.mapUrl("/PegasusReport/Download/?identifier=" + identifier);

                    $("#LoadedExcel #linkExcelDownloaded").attr("href", url)

                    $("#LoadingExcel").hide();
                    $("#LoadedExcel").show();

                }

            }
        });
    }


    self.PivotModal = new PivotViewModel(
        self.CreatePivot
    );


    self.PivotModal.Fields([
        { name: 'Matriz Código SAP', value: 1, type: 'Rows' },
        { name: 'Matriz CNPJ', value: 2, type: 'Rows' },
        { name: 'Matriz Razão Social', value: 3, type: 'Rows' },
        { name: 'Matriz Nome Fantasia', value: 4, type: 'Rows' },
        { name: 'Filial Código SAP', value: 5, type: 'Rows' },
        { name: 'Filial CNPJ', value: 6, type: 'Rows' },
        { name: 'Filial Nome Fantasia', value: 7, type: 'Rows' },
        { name: 'Número Nota Fiscal', value: 8, type: 'Columns' },
        { name: 'Data Emissão Nota Fiscal', value: 9, type: 'Columns' },
        { name: 'Nome Emitente', value: 21, type: 'Columns' },
        { name: 'CPF/CNPJ Emitente', value: 22, type: 'Columns' },
        { name: 'CFOP', value: 10, type: 'Columns' },
        { name: 'Tipo de Transação', value: 11, type: 'Columns' },
        { name: 'CPF/CNPJ Destinatário', value: 12, type: 'Columns' },
        { name: 'Nome Destinatário', value: 13, type: 'Columns' },
        { name: 'Cidade Destinatário', value: 14, type: 'Columns' },
        { name: 'UF Destinatário', value: 15, type: 'Columns' },
        { name: 'Código SAP Produto', value: 16, type: 'Columns' },
        { name: 'Produto', value: 17, type: 'Columns' },
        { name: 'Quantidade Comercializada', value: 18, type: 'Metrics' },
        { name: 'Valor Unitário Comercializado', value: 19, type: 'Metrics' },
        { name: 'Valor Total', value: 20, type: 'Metrics' }
    ]);

    self.Partners = ko.observableArray([]);


    self.Units = ko.observableArray([]);

    self.Clients = ko.observableArray([]);

    self.Brands = ko.observableArray([]);

    self.Products = ko.observableArray([]);

    self.Cities = ko.observableArray([]);

    self.ReportDateViewModel = new ReportDateViewModel();



    self.GenerateExcel = function () {
        util.ValidationResults.ClearErrors();


        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), self.OnGenerateExcel);
        } else {
            self.OnGenerateExcel();
        }
    }



    self.OnGenerateExcel = function () {       
        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        var data = ko.toJSON(vmObj);        
        $("#LoadingExcel").show();
        $("#LoadedExcel").hide();

        $('#ModalExcel').modal('show');
        
        $.ajax({
            type: "POST",
            url: util.mapUrl("/PegasusReport/GenerateExcel"),
            data: data,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    $('#ModalExcel').modal('hide');
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    var identifier = results.Identifier;
                 

                    var url = util.mapUrl("/PegasusReport/Download/?identifier=" + identifier);

                    $("#LoadedExcel #linkExcelDownloaded").attr("href", url)

                    $("#LoadingExcel").hide();
                    $("#LoadedExcel").show();
                    
                }

            }
        });
        
    }

    self.GeneratePivot = function () {
        $("#ModalPivot").modal();
    }

    

    self.SendEmail = function () {
        self.SaveHistory(self.ShowModalEmail);
    }

    self.ShowModalEmail = function () {
       
        $('#ModalEmail').modal();
    }

    self.ProcessEmail = function () {
       
        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        var data = ko.toJSON(vmObj);

        $.ajax({
            type: "POST",
            url: util.mapUrl("/PegasusReport/SendEmail"),
            data: data,
            contentType: "application/json",
            success: function (results) {

                if (results.hasErrors) {
                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    $('#ModalEmail').modal('hide');
                }

            }
        });
    }

}