function CFOPViewModel() {

    var self = this;

    self.CFOPsCredit = ko.observableArray([]);
    self.CFOPsDebit = ko.observableArray([]);

    self.LoadData = function () {

        var data = { search: 'Credito' };
        
        $.ajax({
            type: "POST",
            data: JSON.stringify(data),
            url: util.mapUrl("/CFOP/Results"),
            contentType: "application/json",
            success: function (results) {
                self.CFOPsCredit.removeAll();
                ko.utils.arrayForEach(results, function (item) {
                    self.CFOPsCredit.push(item);
                });
            }
        });

        data = { search: 'Debito' };

        $.ajax({
            type: "POST",
            data: JSON.stringify(data),
            url: util.mapUrl("/CFOP/Results"),
            contentType: "application/json",
            success: function (results) {
                self.CFOPsDebit.removeAll();
                ko.utils.arrayForEach(results, function (item) {
                    self.CFOPsDebit.push(item);
                });
            }
        });

    }
    
    self.LoadData();

    self.ShowModal = function () {
        $('#ModalCFOP').modal();
        $('#ModalCFOP .nav-tabs li').unbind('mouseenter mouseleave hover');
        new SimpleBar($('#ModalCFOP .table-wrapper')[0], {
            autoHide: false
        });
    }
}