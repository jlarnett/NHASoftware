$(document).ready(function() {

            $('#Submit').click(function() { 
            var selectedAuthorList = null;

            selectedAuthorList = [];
            var bookAuthor = {};

            $("input:checkbox:checked").each(function() {
                selectedAuthorList.push($(this).attr('value'));
            });

            bookAuthor.bookId = $('#book').attr('value');
            bookAuthor.authors = selectedAuthorList;

            console.log(bookAuthor);

            $.ajax({
                url: '/api/BookAuthors',
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                datatype: 'json',
                data: JSON.stringify(bookAuthor),
                success: function(data) {
                    if (data.success) {
                        window.location.href = "/books/index";
                    }
                }
            });
         });
    });


