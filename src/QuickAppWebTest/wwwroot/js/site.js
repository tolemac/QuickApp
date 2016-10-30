// Write your Javascript code.

$("#quickAppButton").click(function () {

    var insertData = {
        collectionName: "People",
        document: { name: "Javier", surname: "Ros" }
    };

    $.ajax("/qa/mongodb/InsertOne", {
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        method: "POST",
        data: JSON.stringify(insertData)
    }).success(function (data) {

        var findData = {
            collectionName: "People",
            filter: {}
        };
        $.ajax("/qa/mongodb/Find",
            {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                method: "POST",
                data: JSON.stringify(findData)
            }).success(function (data) {
                for (var i = 0, j = data.length; i < j; i++) {
                    alert(data[i].name + " " + data[i].surname);
                }
            });
    });
});
