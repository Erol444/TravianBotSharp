import { Typography } from "@mui/material";
import React, { useState, useEffect } from "react";
import ContentBox from "../../../ContentBox";
import MUITable from "../../../ref/MUITable";
import { useSelector } from "react-redux";
import { useVillage } from "../../../../hooks/useVillage";
import { getCurrentList } from "../../../../api/api";
const CurrentBuilding = () => {
	const header = ["Building", "Level", "Time"];
	const data = [
		{
			id: 0,
			building: "Loading ...",
			level: 3,
			time: "2022/02/02 02:02:02",
		},
		{
			id: 1,
			building: "Loading ...",
			level: 2,
			time: "2022/02/02 02:02:02",
		},
	];

	const [items, setItems] = useState(data);
	const [selected, setSelected] = useState(0);
	const account = useSelector((state) => state.account.info);
	const [villageId] = useVillage();
	useEffect(() => {
		if (account.id !== -1 && villageId !== -1) {
			getCurrentList(account.id, villageId).then((data) => {
				data.forEach((item) => {
					item.completeTime = new Date(
						item.completeTime
					).toLocaleString("vi-VN");
					// i will leave this here until EriK or me (VINAGHOST) make an option for this like Travian do
					// now it will show "hh:mm:ss dd/mm/yyyy"
				});
				setItems(data);
			});
		}
	}, [account.id, villageId]);
	const onClick = (selected) => {
		setSelected(selected);
	};
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Current Building</Typography>
				<MUITable
					header={header}
					data={items}
					selected={selected}
					onClick={onClick}
				/>
			</ContentBox>
		</>
	);
};

export default CurrentBuilding;
