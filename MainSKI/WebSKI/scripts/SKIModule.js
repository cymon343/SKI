angular.module('SKI', ['ngRoute'])
.constant('apiUrl', 'http://localhost:36172/ProductionOrderService.svc/')
.factory('orderFactory', OrderFactory)
.service('webApi',WebApi)
.controller('skiController', SKICtrl)
//.controller('orderController', OrderCtrl)  

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
    var currentOrder = {};

    return{
        getOrders: function()
        {
            return orders;
        },
        setOrders: function(newObject)
        {
            orders = newObject;

            //console.log("[orders SET]:");
            //console.log(newObject);
        },
        getCurrentOrder: function () {
            return currentOrder;
        },
        setCurrentOrder: function (newObject) {
            currentOrder = newObject;

            //console.log("[currentOrder SET]:");
            //console.log(newObject);
        },
        getOrderbyID: function (ID) {
            for (var i = 0; i < orders.length; i++) {
                if (orders[i].ID == ID) {
                    return orders[i];
                }
            }
            return null;
        },
        getElementsByHeading(heading) {
            var tmpElements = [];

            for (var i = 0; i < currentOrder.Elements.length; i++) {
                if (currentOrder.Elements[i].Heading == heading) {
                    tmpElements.push(currentOrder.Elements[i]);
                }
            }
            //console.log("Returned Elements from getElementsByHeading.");
            //console.log(tmpElements);
            return tmpElements;
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
           url: apiUrl + "getOrders",
           param:'',
           data: ''
       }
        return $http(req);      
    }
    this.setElementComment = function(orderID, elementID, stationNumber, comment)
    {
        //var data = {orderID: "test", elementID: "test", stationNumber: "test", comment: "test"}
        var data = { orderID: orderID, elementID: elementID, stationNumber: stationNumber, comment: comment }
        //var data = [orderID, elementID, stationNumber, comment];
        //TODO: Work from here. Current state - Receives error 400 (Bad Request)
        $http({
            url: apiUrl + "setElementComment",
            method: "POST",
            params: data,
        })

    }
}


//SKIController:
function SKICtrl($scope, orderFactory, webApi)
{  
    $scope.view = 'list';
    $scope.currentProgressObject = {}; //Saving CurrentProgressState rather than just the "comment" part of it for two reasons: 1. We can also access the bools, but secondly, and more important - ng-model does not work with primitives!
    $scope.getOrdersFromService = function () {
        webApi.getAllOrders()
           .then(function successCallback(response) {               
               var jsonObject = response.data.GetOrdersResult;

               console.log("[OBJECT RETRIEVED FROM SERVICE]:");
               console.log(jsonObject);               
               orderFactory.setOrders(jsonObject);               
           }, function errorCallback(response) {
               console.log(response);   //TODO: Handle failure response
           });
    }


    $scope.getOrdersFromService();
    $scope.orderFactory = orderFactory; //Is it OK to bind the factory to the scope?

    $scope.isOrderBegun = function isOrderBegun(id, stationIndex)
    {
        //console.log("printing ID from [BEGUN] method:" + id + " -stationNum: " + stationNum);
        success = false;       
        var theOrder = orderFactory.getOrderbyID(id);

        if (theOrder != null) {
            for (var i = 0; i < theOrder.Elements.length; i++) {
                success = success || theOrder.Elements[i].ProgressInfo[stationIndex].Begun;
                if (success == true) {
                    return success;
                }
            }
        }       
        return success;
    }

    $scope.isOrderDone = function isOrderDone(id, stationIndex)
    {
        //console.log("printing ID from [DONE] method:" + id + " -stationNum: " + stationNum);
        success = true;
        var theOrder = orderFactory.getOrderbyID(id);

        if (theOrder != null) {
            for (var i = 0; i < theOrder.Elements.length; i++) {
                success = success && theOrder.Elements[i].ProgressInfo[stationIndex].Done;
                if (success != true) {
                    return success;
                }
            }
        }
        return success;
    }

    $scope.displayOrderCard = function displayOrderCard(orderID)
    {
        orderFactory.setCurrentOrder(orderFactory.getOrderbyID(orderID));
    }

    $scope.displayAppendixLinkList = function displayAppendixLinkList(orderID)
    {        
        orderFactory.setCurrentOrder(orderFactory.getOrderbyID(orderID));
    }

    function setView(view) 
    {
        $scope.view = view;
    }

    $scope.startEdit = function startEdit(progressInfo)
    {
        $scope.currentProgressObject = progressInfo;
        console.log("Reading Comment:");
        console.log($scope.currentProgressObject.Comment);
        setView('edit');
    }

    $scope.cancelEdit = function cancelEdit()
    {
        setView('list');
    }

    $scope.saveComment = function saveComment()
    {
        setView('list');
        console.log("Comment saved:");
        console.log($scope.currentProgressObject.Comment);
        webApi.setElementComment(orderFactory.getCurrentOrder().orderID, $scope.currentProgressObject.ParentID, $scope.currentProgressObject.StationNumber, $scope.currentProgressObject.Comment);
        //TODO: Send to service.
    }
    
}



































//OrderCtrl:
/*function OrderCtrl($scope) {
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
}*/