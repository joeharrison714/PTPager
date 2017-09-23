metadata {
    definition (name: "PTPager", namespace: "Harrisys", author: "Joe Harrison") {
        capability "Speech Synthesis"
    }
    preferences {
        input("DeviceAddress", "string", title:"PTPager IP Address", description:"IP address of the machine running PTPager", defaultValue:"" , required: true, displayDuringSetup: true)
        input("DevicePort", "string", title:"PTPager Port", description:"Port of the machine running PTPager", defaultValue:"5666", required: true, displayDuringSetup: true)
    }
    tiles {
        standardTile("speak", "device.speech", inactiveLabel: false, decoration: "flat") 
        {
            state "default", label:'Speak', action:"Speech Synthesis.speak", icon:"st.Electronics.electronics13"
        }
    }
}

String getVersion() {return "0.1";}

/** speechSynthesis Capability: Speak
 */
def speak(toSay) {
    log.debug "Executing 'speak'"
    if (!toSay?.trim()) {
        if (ReplyOnEmpty) {
            toSay = "PTPager Version ${version}"
        }
    }

    if (toSay?.trim()) {
        def command="&SPEAK="+toSay+"&"+getDoneString()
        return transmit(command)
    }
}