import React, { useEffect } from "react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import Layout from "./components/Layout";

import { signalRConnection, initConnection } from "./realtime/connection";
import { changeAccount } from "./realtime/account";
import { usePrevious } from "./hooks/usePrevious";
import { useDispatch, useSelector } from "react-redux";

import { HubConnectionState } from "@microsoft/signalr/dist/esm/HubConnection";

const App = () => {
	toast.configure();
	const account = useSelector((state) => state.account.info.id);
	const prev = usePrevious(account);
	const dispatch = useDispatch();
	useEffect(() => {
		initConnection();
		signalRConnection.start().then(() => {
			signalRConnection.on("message", (data) => console.log(data));
		});
	}, [dispatch]);

	useEffect(() => {
		if (signalRConnection.State === HubConnectionState.Connected) {
			changeAccount(account, prev);
		}
	}, [account, prev]);

	return <Layout />;
};

export default App;
