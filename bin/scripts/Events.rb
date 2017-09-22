require 'mscorlib'

class Events

    include Minerva

    def initialize()
        @Globals = IronRuby.globals
    end

	def failed_login(sender, e)
		Log.Notice "Failed login: #{e.Username} - #{e.IP}"
	end

	def _init_(events)
		events.OnFailedLogin { |sender, e| self.failed_login sender, e }
	end

end