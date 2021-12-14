$(document).ready(function ()
{
    var table = $("#authors").DataTable({
        ajax: {
            url: "/api/authors",
            dataSrc: ""
        },
        columns: 
        [
            {
                data: "id"
            },
            {
                data: "name" , 
                render: function(data, type, author) {
                    return "<a href='/authors/edit/" + author.id + "'>" + author.name + "</a>";
                }
            },
            {
                data: "address"
            },
            {
                data: "id",
                render: function (data) 
                {
                    return "<button class='btn-primary js-delete' data-author-id=" + data + ">Delete</button>";
                }
            }
        ]
    });

    $("#authors").on("click", ".js-delete", function () {
        var button = $(this);

        $.ajax ({ 
            url: "/api/authors/" + button.attr("data-author-id"),
            method: "DELETE",
            success: function () 
            {
                table.row(button.parents("tr")).remove().draw();
            }
        });
    });

});