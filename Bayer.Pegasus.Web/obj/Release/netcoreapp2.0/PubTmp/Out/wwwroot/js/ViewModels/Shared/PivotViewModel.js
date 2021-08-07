function PivotViewModel(onNextStep) {

    var self = this;
    
    self.CurrentDrag = ko.observable();

    self.Fields = ko.observableArray();

    self.SelectedIds = ko.observableArray();

    self.SelectedIds.subscribe(function (changes) {
        var data = ko.utils.arrayFirst(self.Fields(), function (item) {
            return item.value == changes[0]["value"];
        });

        if (changes[0]["status"] == "added") {
            var type = (typeof data.type != 'function') ? data.type : data.type();
            self[type].push(data);
            self.SelectedItems.push(data);
            self.Fields.remove(data);
        } else {
            self.Criteria.remove(data);
            self.Columns.remove(data);
            self.Rows.remove(data);
            self.Metrics.remove(data);
            self.SelectedItems.remove(data);
        }
    }, null, "arrayChange");

    self.Clean = function () {
        var idList = ko.toJS(self.SelectedIds());
        ko.utils.arrayForEach(idList, function (e) {
            var data = ko.utils.arrayFirst(self.SelectedItems(), function (item) {
                return item.value == e;
            });
            if (data != null) {
                self.Criteria.remove(data);
                self.Columns.remove(data);
                self.Rows.remove(data);
                self.Metrics.remove(data);
                self.SelectedItems.remove(data);
                self.Fields.push(data);
            }
        });
    }

    self.Fields.subscribe(function (changes) {
        if (changes[0]["status"] == "added") {
            self.SelectedIds.remove(changes[0]["value"].value);
        }
    }, null, "arrayChange");

    self.SelectedItems = ko.observableArray();
    
    self.Criteria = ko.observableArray();
    self.Columns = ko.observableArray();
    self.Rows = ko.observableArray();
    self.Metrics = ko.observableArray();

    self.drop = function (data, model, args) {
        var target = args.element.id.replace('DragDrop', '');
        if (self.CurrentDrag == target) return;
        self[self.CurrentDrag].remove(data);
        self[target].push(data);
    };

    self.dragStart = function (data, args) {
        var target = (typeof args.srcElement == 'undefined') ? args.target : args.srcElement;
        self.CurrentDrag = $(target).parents('.canDrop').attr('id').replace('DragDrop', '');
        $('.drop-zone').not($(target).parents('.canDrop')).addClass('drop-target');
    };

    self.dragEnd = function (data, args) {
        $('.drop-target').removeClass('drop-target');
    }

    self.removeField = function (data, args) {
        var target = (typeof args.srcElement == 'undefined') ? args.target : args.srcElement;
        self.CurrentDrag = $(target).parents('.canDrop').attr('id').replace('DragDrop', '');
        self[self.CurrentDrag].remove(data);
        self['Fields'].push(data);
    }

    self.NextStep = function () {
        onNextStep();
    }

    new SimpleBar($('#DragDropFields')[0], {
        autoHide: false
    })
}