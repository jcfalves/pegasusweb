var ajaxFlow = new function () {

    var self = this;
    
    self.StartLoadingChart = function (selector) {
        $('.panel-body .row.graficos').css('visibility', 'hidden');
        $(selector).find(".grafico").hide();
        $(selector).find(".loader").remove();
        
        $(selector).append("<div class='loader'></div>");
    }

    self.FinishLoadingChart = function (selector) {
        $('.panel-body .row.graficos').css('visibility', 'visible');
        $(selector).find(".grafico").fadeIn();
        $(selector).find(".loader").remove();
        $('.openCFOP').show();
        //util.adjustHeightContent();        
    }
  
}