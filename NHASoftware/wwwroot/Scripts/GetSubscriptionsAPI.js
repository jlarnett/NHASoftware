$(document).ready(function ()
{
    var table = $("#subscriptions").DataTable({
        ajax: {
            url: "/api/Subscriptions",
            dataSrc: ""
        },
        columns: 
        [
            {
                data: "subscriptionId"
            },
            {
                data: "subscriptionName"
            },
            {
                data: "subscriptionDay"
            },
            {
                data: "subscriptionCost"
            },
            {
                data: "subscriptionId",
                render: function (data) 
                {
                    return "<button class='btn-primary js-delete' data-sub-id=" + data + ">Delete</button>" + "<button class='btn-primary js-edit' data-sub-id=" + data + ">Edit</button>";
                },

            }
        ]
    });

    $("#subscriptions").on("click", ".js-delete", function () {
        var button = $(this);

        $.ajax ({ 
            url: "/api/Subscriptions/" + button.attr("data-sub-id"),
            method: "DELETE",
            success: function () 
            {
                table.row(button.parents("tr")).remove().draw();
            }
        });
    });

    $("#subscriptions").on("click", ".js-edit", function () {
        var button = $(this);
        window.location.href = "/subscriptions/edit/" + button.attr("data-sub-id");
    });
});