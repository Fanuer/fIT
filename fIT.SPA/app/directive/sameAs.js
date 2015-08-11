"use strict";

function sameAs() {
    /// <summary>Checks if the value of this element is the same as an other property</summary> 
    var result = {};
    result.require = 'ngModel';
    result.link = function($scope, element, attrs, ngModel) {
        ///   <summary>Checks if the current Value is equal than the definied Property</summary>
        ///   <param name="$scope" type="Scope">Current Controller-Scope</param>
        ///   <param name="element" type="Html-Element">Current Html-Element</param>
        ///   <param name="attrs" type="Array of Attributes">Current Html-element's attributes</param>
        ///   <param name="attrs" type="Array of Attributes">Property to check value against</param>
        ///   <returns type="Boolean" />

        // add this validation to the validation pipeline
        function validate(value) {
            var isValid = $scope.$eval(attrs.sameAs) === value;
            ngModel.$setValidity('same-as', isValid);
            return isValid ? value : undefined;
        }

        ngModel.$parsers.unshift(validate);
        // Force-trigger the parsing pipeline.
        $scope.$watch(attrs.sameAs, function () {
            ngModel.$setViewValue(ngModel.$viewValue);
        });
    }
    return result;
};