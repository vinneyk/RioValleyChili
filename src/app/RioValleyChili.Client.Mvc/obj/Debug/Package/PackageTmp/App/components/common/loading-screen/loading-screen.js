function LoadScreenViewModel(params) {
    var self = this;

    self.isVisible = params.isVisible;
    self.loadMessage = params.displayMessage;
}

// Webpack
module.exports = {
    viewModel: LoadScreenViewModel,
    template: require('./loading-screen.html')
};

