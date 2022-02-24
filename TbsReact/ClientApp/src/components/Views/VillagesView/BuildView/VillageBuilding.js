import { Typography } from "@mui/material";
import React, { useState, useEffect, useContext } from "react";
import ContentBox from "../../../ContentBox";
import MUITable from "../../../ref/MUITable";
import { useSelector } from "react-redux";
import { useVillage } from "../../../../hooks/useVillage";

import { getBuildingList } from "../../../../api/api";
import { BuildingContext } from "./Build";
const VillageBuilding = () => {
	const header = ["Location", "Building", "Level"];

	const account = useSelector((state) => state.account.info);
	const [villageId] = useVillage();
	const [buildingId, setBuildingId] = useContext(BuildingContext);
	const [data, setData] = useState([
		{ id: 0, name: "Loading ...", level: 0 },
		{ id: 1, name: "Loading ...", level: 0 },
		{ id: 2, name: "Loading ...", level: 0 },
		{ id: 3, name: "Loading ...", level: 0 },
		{ id: 4, name: "Loading ...", level: 0 },
	]);
	useEffect(() => {
		if (account.id !== -1 && villageId !== -1) {
			getBuildingList(account.id, villageId).then((data) => {
				const newData = data.map(
					({ underConstruction, ...keepAttrs }) => keepAttrs
				);
				setData(newData);
			});
		}
	}, [account.id, villageId]);

	const onClick = (buildingId) => {
		setBuildingId(buildingId);
	};
	return (
		<>
			<ContentBox>
				<Typography variant="h5">Village Building</Typography>
				<MUITable
					header={header}
					data={data}
					onClick={onClick}
					selected={buildingId}
				/>
			</ContentBox>
		</>
	);
};

export default VillageBuilding;
