$(document).ready(function () {
    $('#CommentText').summernote({
        toolbar: [
        // [groupName, [list of button]]
        ['style', ['style']],
        ['style', ['bold', 'italic', 'underline', 'clear']],
        ['para', ['ul', 'ol']],
        ['color', ['color']],
        ['view', ['codeview']],
        ['insert', ['link', 'picture', 'video']],
        ['table', ['table']]
        ],
    });
});