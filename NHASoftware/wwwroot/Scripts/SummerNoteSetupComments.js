﻿$(document).ready(function () {
    $('#CommentText').summernote({
        height: 400,
        width: 1500,
        toolbar: [
        // [groupName, [list of button]]
        ['style', ['style']],
        ['style', ['bold', 'italic', 'underline', 'clear']],
        ['para', ['ul', 'ol']],
        ['color', ['color']],
        ['view', ['codeview']]
        ],

    });
});