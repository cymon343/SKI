angular.module('SKI', ['ngRoute'])
.constant('apiUrl', 'http://localhost:36172/ProductionOrderService.svc/')
.factory('orderFactory', OrderFactory)
.service('webApi',WebApi)
.controller('skiController', SKICtrl)

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
        },
        getCurrentOrder: function () {
            return currentOrder;
        },
        setCurrentOrder: function (newObject) {
            currentOrder = newObject;
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
        var data = {
            "data": {
                "Comment": comment,
                "ElementID": elementID,
                "OrderID": orderID,
                "StationNumber": stationNumber
            }
        }      
        $http({
            method: "POST",
            url: apiUrl + "setElementComment",                      
            data: data            
        })
         .then(function successCallback(response) {
             console.log("[ElementComment posted with success]:");
             console.log(response);
         }, function errorCallback(response) {
             console.log("[ElementComment not posted]:");
             console.log(response);   //TODO: Handle failure response
         });
    }

    this.flipBegun = function(orderID, elementID, stationNumber)
    {
        var data = {
            "data": {                
                "ElementID": elementID,
                "OrderID": orderID,
                "StationNumber": stationNumber
            }
        }      
        $http({
            method: "POST",
            url: apiUrl + "flipElementBegun",                      
            data: data            
        })
         .then(function successCallback(response) {
             console.log("[Begun bool flipped with success]:");
             console.log(response);
         }, function errorCallback(response) {
             console.log("[Begun bool not flipped]:");
             console.log(response);   //TODO: Handle failure response
         });
    }

    this.flipDone = function(orderID, elementID, stationNumber)
    {
        var data = {
            "data": {
                "ElementID": elementID,
                "OrderID": orderID,
                "StationNumber": stationNumber
            }
        }
        $http({
            method: "POST",
            url: apiUrl + "flipElementDone",
            data: data
        })
           .then(function successCallback(response) {
               console.log("[Done bool flipped with success]:");
               console.log(response);
           }, function errorCallback(response) {
               console.log("[Done bool not flipped]:");
               console.log(response);   //TODO: Handle failure response
           });
    }
}


//SKIController:
function SKICtrl($scope, orderFactory, webApi)
{  
    $scope.view = 'list';
    $scope.currentProgressObject = {};
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

    $scope.refreshPage = function refreshPage() {
        orderFactory.setCurrentOrder(null);
        $scope.getOrdersFromService();
    }
    $scope.refreshPage();
    $scope.orderFactory = orderFactory;

    $scope.isOrderBegun = function isOrderBegun(id, stationIndex)
    {       
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
        else {
            success = false;
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
        $scope.currentNote = progressInfo.Comment;
        setView('edit');
    }

    $scope.cancelEdit = function cancelEdit()
    {
        $scope.currentProgressObject.Comment = $scope.currentNote;
        setView('list'); 
    }

    $scope.saveComment = function saveComment()
    {
        setView('list');
        console.log("Comment saved:");
        console.log($scope.currentProgressObject.Comment);
        webApi.setElementComment(orderFactory.getCurrentOrder().ID, $scope.currentProgressObject.ParentID, $scope.currentProgressObject.StationNumber, $scope.currentProgressObject.Comment)       
           
    }
    
    $scope.flipBegun = function flipBegun(progressInfo)
    {
        $scope.currentProgressObject = progressInfo;
        webApi.flipBegun(orderFactory.getCurrentOrder().ID, $scope.currentProgressObject.ParentID, $scope.currentProgressObject.StationNumber)       
    }

    $scope.flipDone = function flipDone(progressInfo)
    {
        $scope.currentProgressObject = progressInfo;
        webApi.flipDone(orderFactory.getCurrentOrder().ID, $scope.currentProgressObject.ParentID, $scope.currentProgressObject.StationNumber)     
    }

    $scope.formatDate = function(date)
    {
        var outDate = new Date(parseInt(date.substr(6)));
        return outDate;
    }

    $scope.buttonText = function(comment)
    {
        if (comment == "") {
            return "btn btn-info btn";
        }
        else {
            return "btn btn-success btn";
        }
    }
}