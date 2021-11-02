define(['ko'], function() {
    return {
        getCurrentUser: getCurrentUser,
    };

    function getCurrentUser() {
        return {
            name: "Vinney Kelly",
            email: "vinneyk@live.com",
            avatarUri: "https://secure.gravatar.com/avatar/8b5e95cfd2f048a94be32f970967d9ae.jpg?d=https%3a%2f%2fportal.azure.com%2fContent%2fImages%2fMsPortalImpl%2fAvatarMenu_defaultAvatarSmall.png",
        };
    }
})