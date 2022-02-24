import React, { useEffect, useState } from "react";
import { getVillages } from "../../../api/Village";

import { useDispatch, useSelector } from "react-redux";
import MUITable from "../../ref/MUITable";
import ContentBox from "../../ContentBox";

import { useVillage } from "../../../hooks/useVillage";

const VillagesTable = () => {
	const account = useSelector((state) => state.account.info);
	const [villages, setVillages] = useState([
		{ id: 0, name: "Loading ...", coords: { x: 0, y: 0 } },
		{ id: 1, name: "Loading ...", coords: { x: 0, y: 0 } },
		{ id: 2, name: "Loading ...", coords: { x: 0, y: 0 } },
		{ id: 3, name: "Loading ...", coords: { x: 0, y: 0 } },
		{ id: 4, name: "Loading ...", coords: { x: 0, y: 0 } },
	]);

	const [selected, setSelected] = useVillage();

	const dispatch = useDispatch();

	const onClick = (villageId) => {
		setSelected(villageId);
	};

	useEffect(() => {
		if (account.id !== -1) {
			getVillages(account.id).then((data) => {
				if (data.length > 0) {
					data.forEach((item) => {
						item.coordinate = `(${item.coordinate.x} | ${item.coordinate.y})`;
					});
					setVillages(data);
					setSelected(data[0].id);
				}
			});
		}
	}, [selected, dispatch, account.id, setSelected]);

	const header = ["Name", "Coords"];

	return (
		<>
			<ContentBox>
				<MUITable
					header={header}
					data={villages}
					onClick={onClick}
					selected={selected}
				/>
			</ContentBox>
		</>
	);
};

export default VillagesTable;
