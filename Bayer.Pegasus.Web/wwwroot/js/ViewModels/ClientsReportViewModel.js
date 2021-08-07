
var targetSVG = "M9,0C4.029,0,0,4.029,0,9s4.029,9,9,9s9-4.029,9-9S13.971,0,9,0z M9,15.93 c-3.83,0-6.93-3.1-6.93-6.93S5.17,2.07,9,2.07s6.93,3.1,6.93,6.93S12.83,15.93,9,15.93 M12.5,9c0,1.933-1.567,3.5-3.5,3.5S5.5,10.933,5.5,9S7.067,5.5,9,5.5 S12.5,7.067,12.5,9z";

var map;

        

function ClientsReportViewModel() {

    var self = this;

    self.Partners = ko.observableArray([]);

    self.ShouldSave = ko.observable(false);

    self.Totals = ko.observableArray([]);

    self.HistoryViewModel = new HistoryViewModel(this, "ClientsReport");

    self.CFOPViewModel = new CFOPViewModel();
    
    self.SaveHistory = function (callback) {
        if (self.ShouldSave()) {
            var vmObj = ko.toJS(self);
            delete vmObj['HistoryViewModel'];
            self.HistoryViewModel.ShowModal(ko.toJSON(vmObj), callback);
        } else {
            callback();
        }
    }

    self.GenerateMap = function () {
        util.ValidationResults.ClearErrors();

        self.SaveHistory(self.GetDataMap);
    }

    self.GetDataMap = function () {
        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        delete vmObj['CFOPViewModel'];

        var data = ko.toJSON(vmObj);

        if (typeof map != "undefined") map.clear();

        $("#filtro-consulta").hide();
        $("#map-results").fadeIn();

        ajaxFlow.StartLoadingChart("#map-results #mapdiv");
        $("#map-results .map-resumo").hide();


         $.ajax({
            type: "POST",
            url: util.mapUrl("/ClientsReport/DataMap"),
            data: data,
            contentType: "application/json",
            success: function (results) { 

                if (results.hasErrors) {
                    $("#filtro-consulta").fadeIn();
                    $("#map-results").hide();

                    util.ValidationResults.ShowErrors(results);
                }
                else {
                    var localPoints = new Array();

                    self.Totals(results.totalData);

                    ko.utils.arrayForEach(results.locationData, function (item) {

                        var content = {
                            svgPath: targetSVG,
                            scale: 0.5,
                            title: item.title,
                            description:
                            '<ul id="DadosResumoFloater" class="resumo-info">' +
                            '<li><strong>Perdidos</strong>: <span>' + item.qt_lost.toLocaleString('pt-BR') + '</span></li>' +
                            '<li><strong>Leais</strong>: <span>' + item.qt_loyal.toLocaleString('pt-BR') + '</span></li>' +
                            '<li><strong>Retidos</strong>: <span>' + item.qt_retained.toLocaleString('pt-BR') + '</span></li>' +
                            '<li><strong>Adquiridos</strong>: <span>' + item.qt_acquired.toLocaleString('pt-BR') + '</span></li>' +
                            '<li><strong>Readquiridos</strong>: <span>' + item.qt_reacquired.toLocaleString('pt-BR') + '</span></li>' +
                            '</ul>',
                            latitude: item.latitude,
                            longitude: item.longitude
                        };



                        localPoints.push(content);
                    });


                    map = new AmCharts.AmMap(AmCharts.themes.pegasus);

                    map.balloon.enabled = false;
                    map.mouseWheelZoomEnabled = true;
                    map.zoomDuration = 0;

                    var dataProvider = {
                        mapVar: AmCharts.maps.brazilLow,
                        getAreasFromMap: true,
                        images: localPoints
                    };

                    map.dataProvider = dataProvider;

                    map.areasSettings = {
                        selectedColor: "#114960",
                        descriptionWindowLeft: 20
                    };

                    map.centerMap = false;

                    map.write("mapdiv");                    
                    $("#map-results .map-resumo").fadeIn();
                    
                    ajaxFlow.FinishLoadingChart("#map-results #mapdiv");
                    util.adjustHeightContent();
                }
            }
        });

    }
    
    self.GenerateExcel = function () {
        var vmObj = ko.toJS(self);
        delete vmObj['HistoryViewModel'];
        delete vmObj['CFOPViewModel'];

        var data = ko.toJSON(vmObj);

        var url = util.mapUrl("/ClientsReport/Excel/");

        window.open(
            url + '?json=' + data,
            '_blank'
        );
    }
}