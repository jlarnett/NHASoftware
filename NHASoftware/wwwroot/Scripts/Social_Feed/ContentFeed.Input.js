class ContentFeedInput {

    static GetInputForm(inputType = "basic", uuid = undefined, parentPostId = undefined) {

        if (inputType === "basic") {
            let form = document.getElementById("BasicPostForm");
            let formData = new FormData(form);
            formData.set("Summary", $($("#MainPostTextbox").summernote("code")).text())
            return formData;
        }

        if (inputType === "custom") {
            let form = document.getElementById("CustomPostForm");
            let formData = new FormData(form);
            formData.set("Summary", $($("#CustomPostTextbox").summernote("code")).text())
            return formData;
        }

        if (inputType === "comment" && uuid !== undefined && parentPostId !== undefined) {
            let form = document.querySelector('[comment-form-uuid="' + uuid + '"]');
            let formData = new FormData(form);

            let commentTextboxJqueryLocator = '[comment-textbox-uuid="' + uuid + '"]'
            let commentText  = $(commentTextboxJqueryLocator).summernote('code').replace(/<\/p>/gi, "\n")
                    .replace(/<br\/?>/gi, "\n")
                    .replace(/<\/?[^>]+(>|$)/g, "")
                    .replace(/&nbsp;|<br>/g, ' ');

            formData.set("ParentPostId", parentPostId);
            formData.set("Summary", commentText);
            return formData;
        }
    }

    static ClearCustomPostForm() {
        $("#CustomPostTextbox").summernote('reset');
        $("#CustomPostImageFileInput").val(null);
        $("#CustomPostValidationMessage").hide("slow");
    }

    static ClearBasicPostForm() {
        $("#MainPostTextbox").summernote('reset');
        $("#MainPostTextboxValidationMessage").hide("slow");
    }

    static ClearCommentForm(uuid) {
        var commentTextboxJqueryLocator = '[comment-textbox-uuid="' + uuid + '"]';
        $(commentTextboxJqueryLocator).summernote("reset");
        $("span[unique-error-identifier$="+ uuid +"]").hide("slow");
    }
}