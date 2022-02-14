import React, { useEffect } from "react";

import ContentBox from "../../ContentBox";

const ActivitySettings = () => {
	useEffect(() => {
		console.log("ActivitySettings");
	}, []);
	return (
		<>
			<ContentBox name="activity" />
		</>
	);
};
export default ActivitySettings;
