var graphEvolucao;
var graphEstoque;
var tickFont = {
    color: "#6C6C6A",
    family: "Neue Helvetica W01",
    weight: 500
};
var prop = {
    animationDuration: 1000,
    gridColor: "#EBEBEB"
};
var labelForAxis = {
    Quantity: "Quantidade/L",
    Value: "Valor (R$)"
}

$(document).ready(function () {
    $('.nav>li').on('mouseenter mouseleave', function () {
        $(this).children('a').click();
    });


    $('#headerFiltro #tabs-panel .tab, #chart-results h2.title-tab, #report-results h2.title-tab, #map-results h2.title-tab').click(function () {
        $('#tabs-panel .tab').removeClass('active');
        $('.panel-filtros').hide();
        $('#' + $(this).attr('data-open')).fadeIn();
        $('#tabs-panel .tab[data-open="' + $(this).attr('data-open') + '"]').addClass('active');
        util.adjustHeightContent();
    });

    $('span.collapse-graph').click(function () {
        $(this).parent().parent().find('.grafico').slideToggle();
        $(this).toggleClass('fa-chevron-up');
        $(this).toggleClass('fa-chevron-down');
    });

    $('span.filter-graph').click(function () {
        var modal = $(this).attr('data-graph');

        openModal("#modal" + modal);
    });

    $('.print-tab').click(function () {
        window.print();
    });

    $('#buttonsRelatorio button').click(function () {
        $('.openCFOP').hide();
    });
});

function openModal(modal) {

    $(modal).modal();

    if (window.top.document.querySelector('iframe')) {
        var ajusteScroll = $(window.parent.document).find('html').scrollTop() - $(window.parent.document).find('#menu').height();
        $(modal).css('top', (ajusteScroll > 0) ? ajusteScroll : 0);
    } else {
        $(modal).css('top', 20);
    }
}

function createChartEvolutionSellOut(selector, chartData, ticks, animate, tooltips, typeDataChart) {
    animate = animate || false;
    $('.row.graficos .wrap-grafico').css('overflow', 'visible');
    accounting.settings.currency.format = "%v";
    accounting.settings.currency.decimal = ",";  // decimal point separator
    accounting.settings.currency.thousand = ".";  // thousands separator
    accounting.settings.currency.precision = 0;   // decimal places 

    var optionsEvolucao = {
        series: {
            grow: {
                active: animate,
                duration: prop['animationDuration']
            },
            lines: {
                show: true,
                lineWidth: 3,
                fill: 1
            },
            points: {
                show: true,
                radius: 5
            },
            hoverable: true
        },
        valueLabels: {
            show: true
        },
        grid: {
            borderColor: {
                left: "#FFF",
                top: prop['gridColor'],
                right: "#FFF",
                bottom: "#EBEBEB"
            },
            borderWidth: 1,
            margin: {
                left: 10,
                right: 0
            },
            hoverable: true
        },
        axisLabels: {
            show: false
        },
        xaxis: {
            color: "rgba(255,255,255,0)",
            ticks: ticks,
            font: tickFont
        },
        yaxis: {
            color: "#EBEBEB",
            font: tickFont,
            tickFormatter: formatter
        },
        hooks: {
            draw: posicionarLabels
        },
        tooltip: {
            show: true,
            content: function (label, xval, yval, flotItem) {
                return parseFloat(tooltips[flotItem.seriesIndex]["data"][flotItem.dataIndex][1].toFixed(2)).toLocaleString('pt-BR');
            },
            defaultTheme: false
        }
    };

    $(selector).attr('data-animate', animate);

    graphEvolucao = $.plot(selector, chartData, optionsEvolucao);

    util.createAxisLegend($(selector), typeDataChart);
}

function createChartEvolutionStock(selector, chartData, ticks, animate, tooltips, typeDataChart) {
    animate = animate || false;
    $('.row.graficos .wrap-grafico').css('overflow', 'visible');
    accounting.settings.currency.format = "%v";
    accounting.settings.currency.decimal = ",";  // decimal point separator
    accounting.settings.currency.thousand = ".";  // thousands separator
    accounting.settings.currency.precision = 0;   // decimal places 

    var optionsEstoque = {
        series: {
            grow: {
                active: animate,
                duration: prop['animationDuration']
            },
            lines: {
                show: true,
                lineWidth: 3
            },
            points: {
                show: true,
                radius: 5
            }
        },
        valueLabels: {
            show: true
        },
        grid: {
            borderColor: {
                left: "#FFF",
                top: "#EBEBEB",
                right: "#FFF",
                bottom: "#EBEBEB"
            },
            borderWidth: 1,
            margin: {
                left: 10,
                right: 0
            },
            hoverable: true
        },
        xaxis: {
            color: "rgba(255,255,255,0)",
            ticks: ticks,
            font: tickFont
        },
        yaxis: {
            color: "#EBEBEB",
            font: tickFont,
            tickFormatter: formatter
        },
        hooks: {
            draw: posicionarLabels
        },
        legend: {
            show: true,
        },
        tooltip: {
            show: true,
            content: function (label, xval, yval, flotItem) {
                return parseFloat(tooltips[flotItem.seriesIndex]["data"][flotItem.dataIndex][1].toFixed(2)).toLocaleString('pt-BR');
            },
            defaultTheme: false
        }
    };

    $(selector).attr('data-animate', animate);

    graphEstoque = $.plot(selector, chartData, optionsEstoque);

    util.createAxisLegend($(selector), typeDataChart);
}

function createChartSellOutStock(selector, chartData, ticks, animate, tooltips, typeDataChart) {
    animate = animate || false;
    $('.row.graficos .wrap-grafico').css('overflow', 'visible');

    var options = {
        series: {
            grow: {
                active: animate,
                duration: prop['animationDuration']
            },
            lines: {
                show: true,
                lineWidth: 3
            },
            points: {
                show: true,
                radius: 5
            }
        },
        grid: {
            borderColor: {
                left: "#FFF",
                top: "#EBEBEB",
                right: "#FFF",
                bottom: "#EBEBEB"
            },
            borderWidth: 1,
            margin: {
                left: 10,
                right: 0
            },
            hoverable: true
        },
        xaxis: {
            color: "rgba(255,255,255,0)",
            ticks: ticks,
            font: tickFont
        },
        yaxis: {
            color: "#EBEBEB",
            font: tickFont,
            tickFormatter: formatter
        },
        hooks: {
            draw: posicionarLabels
        },
        tooltip: {
            show: true,
            content: function (label, xval, yval, flotItem) {
                var seriesName = "stock";
                var indexType = "dataIndex";
                if (flotItem.seriesIndex == 1) {
                    seriesName = "sellout";
                    indexType = "dataIndex";
                }

                if (typeof tooltips[seriesName] == 'undefined')
                    return;

                return parseFloat(tooltips[seriesName][flotItem[indexType]][1].toFixed(2)).toLocaleString('pt-BR');
            },
            defaultTheme: false
        }
    };

    valuesEstoque = chartData.stock;
    valuesSellOut = chartData.sellout;

    var data = [
        {
            lines: {
                show: true
            },
            label: "Estoque",
            data: valuesEstoque,
            color: '#C0B6C1'
        },
        {
            lines: {
                show: false
            },
            points: {
                show: false,
            },
            label: "Sell Out",
            data: valuesSellOut,
            color: '#9BD4F2',
            bars: {
                show: true,
                barWidth: 0.1,
                fillColor: "#9BD4F2",
                fill: true
            }
        }
    ];

    $(selector).attr('data-animate', animate);

    var graphEstoque = $.plot($(selector), data, options);

    util.createAxisLegend($(selector), typeDataChart);
}

function posicionarLabels(plot, canvascontext) {
    var tableLabels = $(canvascontext.canvas).parent().find('table').clone();
    var classDestino;
    $(tableLabels).removeAttr('style');
    $(tableLabels).addClass('label-grafico');
    if ($('.title-grafico').length > 0) {
        classDestino = '.title-grafico';
    } else {
        classDestino = '.wrap-grafico';
    }
    $(canvascontext.canvas).parents('.evolucao').find(classDestino).find("table").hide();
    var table = $(canvascontext.canvas).parents('.evolucao').find(classDestino).append(tableLabels);
    if (classDestino == '.title-grafico' && $(table).children('.label-grafico').width() > ($('.title-grafico').width() - $('.title-tab').width() - 40)) {
        $(table).children('.label-grafico').css('margin-top', 0);
    }

    $('.row.graficos .wrap-grafico').css('overflow', 'auto');
}

function formatter(val, axis) {
    return parseInt(val.toFixed(axis.tickDecimals)).toLocaleString('pt-BR');
}

function selectStatus(filtro) {
    document.getElementById("statusacao").value = filtro;
    $('#DatatableCFOPRegistration').DataTable().clear().draw();
}

/* Portuguese initialisation for the jQuery UI date picker plugin. */
jQuery(function ($) {
    $.datepicker.regional['pt'] = {
        closeText: 'Fechar',
        prevText: '<Anterior',
        nextText: 'Seguinte',
        currentText: 'Hoje',
        monthNames: ['Janeiro', 'Fevereiro', 'Mar&ccedil;o', 'Abril', 'Maio', 'Junho',
            'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
        monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
            'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
        dayNames: ['Domingo', 'Segunda-feira', 'Ter&ccedil;a-feira', 'Quarta-feira', 'Quinta-feira', 'Sexta-feira', 'S&aacute;bado'],
        dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'S&aacute;b'],
        dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'S&aacute;b'],
        weekHeader: 'Sem',
        dateFormat: 'dd/mm/yy',
        firstDay: 0,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['pt']);
});