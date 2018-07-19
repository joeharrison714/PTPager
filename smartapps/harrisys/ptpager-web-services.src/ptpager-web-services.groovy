/**
 *  PTPager Web Services
 *
 */
 
 import groovy.json.JsonBuilder
 
definition(
    name: "PTPager Web Services",
    namespace: "Harrisys",
    author: "Joe Harrison",
    description: "PTPager web services",
    category: "",
    iconUrl: "https://s3.amazonaws.com/smartapp-icons/Convenience/Cat-Convenience.png",
    iconX2Url: "https://s3.amazonaws.com/smartapp-icons/Convenience/Cat-Convenience@2x.png",
    iconX3Url: "https://s3.amazonaws.com/smartapp-icons/Convenience/Cat-Convenience@2x.png",
    oauth: [displayName: "PTPager Web Services", displayLink: "http://localhost:4567"])


preferences {
  section ("Allow PTPager to control these things...") {
    input "switches", "capability.switch", multiple: true, required: true
  }
}

mappings {
  path("/switches") {
    action: [
      GET: "listSwitches"
    ]
  }
  path("/switches/:command") {
    action: [
      PUT: "updateSwitches"
    ]
  }
  path("/routines") 							{   action: [   GET: "listRoutines"        														]}
  path("/routines/:id") 						{   action: [   GET: "listRoutines",            	POST: "executeRoutine"        				]}
}

/****************************
* Routine API Commands
****************************/

/**
* Gets Routines for location, if params.id is provided, get details for that Routine
*
* @param params.id is the routine id
* @return renders json
*/
def listRoutines() {
	debug("listRoutines called")
    def id = params?.id
    def results = []
    // if there is an id parameter, list only that routine. Otherwise list all routines in location
    if(id) {
        def routine = location.helloHome?.getPhrases().find{it.id == id}
        def myRoutine = [:]
        if(!routine) {
            httpError(404, "Routine not found")
        } else {
            render contentType: "text/json", data: new JsonBuilder(getRoutine(routine)).toPrettyString()            
        }
    } else {
        location.helloHome?.getPhrases().each { routine ->
            results << getRoutine(routine)
        }
        debug("Returning ROUTINES: $results")
        render contentType: "text/json", data: new JsonBuilder(results).toPrettyString()
    }
}

/**
* Executes Routine for location
*
* @param params.id is the routine id
* @return renders json
*/
def executeRoutine() {
	debug("executeRoutine called")
    def id = params?.id
    def routine = location.helloHome?.getPhrases().find{it.id == id}
    if(!routine) {
        httpError(404, "Routine not found")
    } else {
        debug("Executing Routine: $routine.label in location: $location.name")
        location.helloHome?.execute(routine.label)
        render contentType: "text/json", data: new JsonBuilder(routine).toPrettyString()
    }
}

// returns a list like
// [[name: "kitchen lamp", value: "off"], [name: "bathroom", value: "on"]]
def listSwitches() {

    def resp = []
    switches.each {
        resp << [name: it.displayName, value: it.currentValue("switch")]
    }
    return resp
}

void updateSwitches() {
    // use the built-in request object to get the command parameter
    def command = params.command

    // all switches have the comand
    // execute the command on all switches
    // (note we can do this on the array - the command will be invoked on every element
    switch(command) {
        case "on":
            switches.on()
            break
        case "off":
            switches.off()
            break
        default:
            httpError(400, "$command is not a valid command for all switches specified")
    }

}
def installed() {}

def updated() {}