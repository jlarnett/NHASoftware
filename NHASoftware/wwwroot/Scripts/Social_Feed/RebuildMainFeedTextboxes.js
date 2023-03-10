function RebuildFeedTextboxes() {
//Used to rebuild summernote text boxes. This is needed whenever the textboxes are added dynamically to feed.
    $('.summernote-comments').summernote({
        height: 60,
        width: 400,
        toolbar: [
        // [groupName, [list of button]]
        ],
        disableResizeEditor: true,

    });

    $('#MainPostTextbox').summernote({
        height: 60,
        toolbar: [
        // [groupName, [list of button]]
        ],
        disableResizeEditor: true,

    });
}