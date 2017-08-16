$(function () {
    //Create a new communication object that talks to the host page.
    var message = IP.frameMessaging();

    var getModel = function () {
        var model = {
            SocialMediaCustodianArtifactID: $('#socialMediaCustodian').val(),
            SocialMediaType: $('#socialMediaSource').val(),
            NumberOfPostsToReveive: $('#numberOfPostsToRetreive').val(),
            JobIdentifier: $('#jobIdentifier').val(),
            WorkspaceArtifactID: $('#workspaceArtifactID').val(),
    }
        return model;
    };

    //An event raised when the user has clicked the Next or Save button.
    message.subscribe('submit', function () {
        //Execute save logic that persists the state

        var serializedModel = JSON.stringify(getModel());
        this.publish("saveState", serializedModel);
        //Communicate to the host page that it to continue.
        this.publish('saveComplete', serializedModel);
    });

    //An event raised when a user clicks the Back button.
    message.subscribe('back', function () {
        //Execute save logic that persists the state.
        var serializedModel = JSON.stringify(getModel());
        this.publish('saveState', serializedModel);
    });

    //An event raised when the host page has loaded the current settings page.
    message.subscribe('load', function (model) {
        // Set field Value only when model contains value
        if (model.length > 0 && JSON.parse(model) !== null) {
            var localModel = JSON.parse(model);
            $('#socialMediaCustodian').val(localModel.SocialMediaCustodianArtifactID);
            $('#socialMediaSource').val(localModel.SocialMediaType);
            $('#numberOfPostsToRetreive').val(localModel.NumberOfPostsToReveive);
            $('#jobIdentifier').val(localModel.JobIdentifier);
            $('#workspaceArtifactID').val(localModel.WorkspaceArtifactID);
        }
    });
});
