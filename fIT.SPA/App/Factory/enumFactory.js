function enumFactory(cachedHttp, baseUrl, entityNames) {
    var factory = {};
    
    factory.getFitnessTypes = function () {
        return cachedHttp.get(baseUrl + "enums/fitnessTypes", entityNames.fitness);
    }
    factory.getJobTypes = function () {
        return cachedHttp.get(baseUrl + "enums/jobTypes", entityNames.jobs);
    }
    factory.getGenderTypes = function () {
        return cachedHttp.get(baseUrl + "enums/genderTypes", entityNames.gender);
    }

    return factory;
}