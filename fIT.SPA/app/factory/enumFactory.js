function enumFactory($http, baseUrl) {
    var factory = {};

    factory.getFitnessTypes = function () {
        return $http.get(baseUrl + "enums/fitnessTypes");
    }
    factory.getJobTypes = function () {
        return $http.get(baseUrl + "enums/jobTypes");
    }
    factory.getGenderTypes = function () {
        return $http.get(baseUrl + "enums/genderTypes");
    }

    return factory;
}