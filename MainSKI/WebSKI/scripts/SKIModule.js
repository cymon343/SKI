angular.module('SKI', ['ngRoute'])
.constant('apiUrl', 'http://localhost:36172/ProductionOrderService.svc/getOrders')
.factory('orderFactory', OrderFactory)
.service('webApi',WebApi)
.controller('skiController', SKICtrl)
.controller('orderController', OrderCtrl)  

//Config for URL routing (ngRoute).
.config(function ($routeProvider) {
    $routeProvider.when('/orderCard', {
        templateUrl: 'views/orderCard.html'
    });
    $routeProvider.when('/appendixLinks', {
        templateUrl: 'views/appendixLinkList.html'
    });    
    $routeProvider.otherwise({
        templateUrl: 'views/orderList.html'
    });
});

//OrderFactory
function OrderFactory() 
{
    var orders = [];

    return{
        getOrders: function()
        {
            return orders;
        },
        setOrders: function(newObject)
        {
            orders=newObject;
        },
        getOrderByID(ID)
        {
            
        }
    };
}


function WebApi($http, apiUrl)
{    
    this.getAllOrders = function() 
    {
        var req = 
       {
           method: 'GET',
           url: apiUrl,
           param:'',
           data: ''
       }
        return $http(req);
    }    
}


//SKIController:
function SKICtrl($scope, orderFactory, webApi)
{
    $scope.orderFactory = orderFactory;

    orderFactory.setOrders(webApi.getAllOrders());    

    $scope.isOrderBegun = function isOrderBegun(id, stationNum)
    {
        success = false;       
        for (var i = 0; i < $scope.orders.length; i++)
        {
            if ($scope.orders[i].ID = id)
            {
                for (var j = 0; j < $scope.orders[i].Elements.length; j++)
                {
                    success = success || $scope.orders[i].Elements[j].ProgressInfo[stationNum].Begun;
                    if (success == true) {
                        return success;
                    }
                }
                break;
            }            
        }
        return success;
    }

    $scope.isOrderDone = function isOrderBegun(id, stationNum) {
        success = true;

        for (var i = 0; i < $scope.orders.length; i++) {
            if ($scope.orders[i].ID = id)
            {
                for (var j = 0; j < $scope.orders[i].Elements.length; j++)
                {
                    success = success && $scope.orders[i].Elements[j].ProgressInfo[stationNum].Done;
                    if (success == false)
                    {
                        return success;
                    }
                }
                break;
            }
        }
        return success;
    }

    $scope.displayOrderCard = function displayOrderCard(orderID)
    {
        $scope.currentOrder = getOrderbyID(orderID); 
    }

    $scope.displayAppendixLinkList = function displayAppendixLinkList(orderID)
    {        
        $scope.currentOrder = getOrderbyID(orderID);        
    }

    function getOrderbyID(orderID)
    {
        for (var i = 0; i < $scope.orders.length; i++)
        {
            if ($scope.orders[i].ID == orderID)
            {
                return $scope.orders[i];
            }
        }
    }
}


//OrderCtrl:
function OrderCtrl($scope) {
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