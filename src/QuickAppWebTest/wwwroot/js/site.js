// Write your Javascript code.

function qaAjax(service, method, arguments) {
    return $.ajax("/qa/" + service + "/" + method, {
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        method: "POST",
        data: JSON.stringify(arguments)
    })
}

$("#quickAppButton").click(function () {

    qaAjax("auth", "Login", { name: "user1", password: "pwd1" }).success(function () {
        var insertData = {
            collectionName: "People",
            document: { name: "Javier", surname: "Ros" }
        };

        qaAjax("mongodb", "InsertOne", insertData).success(function (data) {
            var findData = {
                collectionName: "People",
                filter: {},
                take: 2
            };
            qaAjax("mongodb", "Find", findData).success(function (data) {
                for (var i = 0, j = data.length; i < j; i++) {
                    alert(data[i].name + " " + data[i].surname + (data[i].userId ? " user id:" + data[i].userId : ""));
                }

                qaAjax("auth", "Logoff");
            });
        });
    });

    
});
