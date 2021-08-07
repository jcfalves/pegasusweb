function ReportDateViewModel(modal) {

    var self = this;
   
    self.StartDate = ko.observable("");

    self.EndDate = ko.observable("");


    self.TypeDate = ko.observable("l");

    self.LastDate = ko.observable("1");

    self.LastType = ko.observable("y");

    self.YearDate = ko.observable("");

    self.CropDate = ko.observable("");

    self.BiMonthDate = ko.observable("");

    self.MonthDate = ko.observable("");

    self.WeekDate = ko.observable("");

    self.IsYearDate = function () {
        return self.TypeDate() == "y";
    }

    self.IsCropDate = function () {
        return self.TypeDate() == "c";
    }

    self.IsBiMonthDate = function () {
        return self.TypeDate() == "b";
    }

    self.IsMonthDate = function () {
        return self.TypeDate() == "m";
    }

    self.IsLastDate = function () {
        return self.TypeDate() == "l";
    }

    self.IsCustomDate = function () {
        return self.TypeDate() == "custom";
    }

    self.TypeDate.subscribe(function(){
        $('.validationError').removeClass('validationError');
        $('.validationMessage').remove();
    });
}