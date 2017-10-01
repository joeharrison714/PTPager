metadata {
    definition (name: "PTPager", namespace: "Harrisys", author: "Joe Harrison") {
        capability "Speech Synthesis"
    }
    preferences {
        input("DeviceAddress", "string", title:"PTPager IP Address", description:"IP address of the machine running PTPager", defaultValue:"" , required: true, displayDuringSetup: true)
        input("DevicePort", "string", title:"PTPager Port", description:"Port of the machine running PTPager", defaultValue:"5566", required: true, displayDuringSetup: true)
        input("PagingChannel", "string", title:"PTPager Channel", description:"Channel to send pages on", defaultValue:"1", required: true, displayDuringSetup: true)
    }
    tiles {
        standardTile("speak", "device.speech", inactiveLabel: false, decoration: "flat") 
        {
            state "default", label:'Speak', action:"Speech Synthesis.speak", icon:"st.Electronics.electronics13"
        }
    }
}

String getVersion() {return "0.3";}

/** speechSynthesis Capability: Speak
 */
def speak(toSay) {
    log.debug "Executing 'speak'"
    if (!toSay?.trim()) {
        
            toSay = "PTPager Version ${version}"
    }

    if (toSay?.trim()) {
        def command="/speak"
        
        def query = [
        	tosay: toSay
        ]
        return transmit(command, query)
    }
}

private transmit(commandString, query) {
    log.info "Sending command "+ commandString+" to "+DeviceAddress+":"+DevicePort
    
    if (DeviceAddress?.trim()) {
        device.deviceNetworkId = "$hosthex:$porthex"
        log.info "deviceNetworkId: " + "$hosthex:$porthex"
        
        def result = new physicalgraph.device.HubAction(
            method: "GET",
            path: "/api" + commandString,
            headers: [
                HOST: "$DeviceAddress:$DevicePort"
            ],
            query: query
        )
		return result;
    }
}

def parse(description) {
	log.info "in parse method"
    def msg = parseLanMessage(description) 
    def headersAsString = msg.header // => headers as a string
    def headerMap = msg.headers      // => headers as a Map
    def body = msg.body              // => request body as a string
    def status = msg.status          // => http status code of the response
    def json = msg.json              // => any JSON included in response body, as a data structure of lists and maps
    def xml = msg.xml                // => any XML included in response body, as a document tree structure
    def data = msg.data              // => either JSON or XML in response body (whichever is specified by content-type header in response)
    
    log.info headersAsString
    log.info status
    log.info body
}