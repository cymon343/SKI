angular.module('main', ['ngRoute'])
  
.controller('movieCtrl', MovieCtrl)
.controller('siteEditCtrl', SiteEditCtrl)


  
//Config for URL routing (ngRoute) ie Switching views from main page.
.config(function ($routeProvider) {
    $routeProvider.when('/movies', {
      templateUrl: 'views/movies.html'
    });
    $routeProvider.when('/contact', {
      templateUrl: 'views/contact.html'
    });
    $routeProvider.otherwise({
      templateUrl: 'views/main.html'
    });
  });  

  //Empty MovieCtrl
function MovieCtrl($scope)
{}


//SiteEditCtrl as a real controller for binding the Model to the view and vice versa. 
function SiteEditCtrl($scope) {
  $scope.movies = movies;
  $scope.startAdd = startAdd;
  $scope.cancel = cancel;
  $scope.add = add;
  $scope.startEdit = startEdit;
  $scope.save = save;
  $scope.startRemove = startRemove;
  $scope.remove = remove;
  $scope.getSelected = getSelected;
  $scope.MovieTitle = '';
  $scope.MovieActor = '';
  $scope.MovieDuration = '';
  

  var selected = -1;
  setView('list');

  function setView(view) {
    $scope.view = view;
  }

  function startAdd() {
    $scope.MovieTitle = '';
    $scope.MovieActor = '';
    $scope.MovieDuration = '';
    setView('add');
  }

  function cancel() {
    setView('list');
  }

  function add() {      
    $scope.movies.push({"title": $scope.MovieTitle, "actor": $scope.MovieActor, "time": $scope.MovieDuration}); 
    setView('list');
  }

  function startEdit(index) {    
    selected = index;
    
    $scope.MovieTitle = $scope.movies[index].title;   
    $scope.MovieActor = $scope.movies[index].actor;  
    $scope.MovieDuration = $scope.movies[index].time;     
      
    setView('edit');   
  }

  function save() {
      
    $scope.movies[selected].title = $scope.MovieTitle;
    $scope.movies[selected].actor = $scope.MovieActor;
    $scope.movies[selected].time = $scope.MovieDuration;
    
    setView('list');
  }

  function startRemove(index) {
    selected = index;
    setView('delete');
  }

  function remove() {
    $scope.movies.splice(selected, 1);
    setView('list');
  }

  function getSelected() {
    return movies[selected].title;
  }
}