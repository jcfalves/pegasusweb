function HistoryViewModel(parent, report) {

    var self = this;

    self.OnSaving = false;

    self.Parent = parent;

    self.Report = report;

    self.Name = ko.observable("");

    self.Items = ko.observableArray([]);

    self.Data = ko.observable();

    self.Callback = ko.observable();

    self.UpdateData = function (results) {
        self.Items.removeAll();

        ko.utils.arrayForEach(results, function (item) {

            self.Items.push(item);
        });
    }

    self.ShowModal = function (data, callback) {
        self.Data = data;
        self.Callback = callback;
        $('#ModalAnalise').modal();
    }

    self.Delete = function (data) {

        util.ShowConfirm("Deseja realizar a exclusão?", function () {

            $.ajax({
                type: "POST",
                url: util.mapUrl("/History/Delete/" + data.id),
                data: JSON.stringify(data),
                contentType: "application/json",
                success: function (result) {
                    console.log(result);
                    if (result.hasErrors) {
                        util.ValidationResults.ShowErrors(result);;
                    }
                    else {
                        self.LoadData();
                    }
                }
            });
        });       
    }

    self.Save = function () {

        if (self.OnSaving)
            return;

        util.ValidationResults.ClearErrors();

	    var vmObj = ko.toJS(self.Data);
        delete vmObj['__ko_mapping__'];
        delete vmObj['ChartData'];


        var data = { description: self.Name(), json: vmObj, report: report, overwrite: false };


        self.SaveHistoryData(data);
    }

    self.SaveHistoryData = function (data) {
        self.OnSaving = true;

        $.ajax({
            type: "POST",
            url: util.mapUrl("/History/Save"),
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (result) {

                self.OnSaving = false;

                if (result.results && result.results.exists) {

                    $('#ModalAnalise').modal('hide');

                    util.ShowConfirm("Histórico existente. Deseja substitui-lo?", function () {
                        data.overwrite = true;
                        self.SaveHistoryData(data);

                        if (typeof self.Callback != "undefined")
                            self.Callback();
                    });


                }
                else {
                    console.log(result);
                    if (result.hasErrors) {
                        util.ValidationResults.ShowErrors(result);;
                    }
                    else {
                        if (typeof self.Callback != "undefined")
                            self.Callback();
                        data.created = new Date(Date.now()).toLocaleString().split(' ')[0];
                        data.id = result;
                        $('#ModalAnalise').modal('hide');
                        self.LoadData();
                    }
                }
            }
        });

    }


    self.LoadData = function () {

        var data = {Report: report};
        
        $.ajax({
            type: "POST",
            url: util.mapUrl("/History/Results"),
            data: JSON.stringify(data),
            contentType: "application/json",
            success: function (results) {
                
                self.UpdateData(results);
            }
        });

    }

    self.LoadHistoric = function (data, item) {

        var data = { Id: item.id };
        var mapping = {
            'ignore': ["HistoryViewModel"]
        }

        $('.datepicker').each(function () {
            $(this).data("DateTimePicker").clear();
        });
        var parsed = JSON.parse(item.json);
        parsed['ShouldSave'] = false;
        console.log(parsed);
        var mappingOptions = {
            'ignore': ["HistoryViewModel", "PivotModal", "__ko_mapping__"]
        };

        ko.mapping.fromJS(parsed, mappingOptions, self.Parent);

        if (typeof parsed.PivotModal != 'undefined') {
            if (parsed.PivotModal.SelectedIds.length > 0) {
                self.Parent.PivotModal.Clean();
                parsed.PivotModal.SelectedIds.forEach(function (e) {
                    self.Parent.PivotModal.SelectedIds.push(e);
                });
            }
            else {
                self.Parent.PivotModal.Clean();
            }
        }

        $('#tabs-panel div[data-open="filtro-consulta"]').click();
    }
}