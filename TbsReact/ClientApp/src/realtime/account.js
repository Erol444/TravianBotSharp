const changeAccount = (signalRConnection, index, oldIndex) => {
	if (index !== -1) {
		signalRConnection.invoke("AddGroup", index);
	}
	if (oldIndex !== -1) {
		signalRConnection.invoke("RemoveGroup", oldIndex);
	}
};

export { changeAccount };
