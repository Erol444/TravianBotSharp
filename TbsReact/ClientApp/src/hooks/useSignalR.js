import { createContext, useContext } from "react";

const SignalRContext = createContext();

const useSignalR = () => {
	const context = useContext(SignalRContext);

	if (!context) {
		throw new Error("useSignalR must be used within a <App />");
	}

	return context;
};

export { SignalRContext, useSignalR };
