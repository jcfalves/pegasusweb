var dataJson;

$.ajax({
    url: util.mapUrl("/Retroativo/GetArquivoRetroativos"),
    dataType: 'json',
    contentType: 'application/json; charset=utf-8',
    method: 'GET',
    success: function (result) {
        dataJson = JSON.parse(result.data);
        console.log(JSON.stringify(dataJson));
        alert(JSON.stringify(dataJson));
        return new TableFeed(JSON.stringify(dataJson));
    }
});

function TableFeed(dataJson) {
    $('#example').DataTable({
        "ajax": dataJson,
        "columns": [
            { "data": "IdArquivoRetroativo" },
            { "data": "Nome" },
            { "data": "Acao" },
            { "data": "IdAcao" },
            { "data": "IdArquivo" }
        ]
        //,
        //"language": {
        //    "url": "../../json/Portuguese-Brasil.json"
        //}
    });
}