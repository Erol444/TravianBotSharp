import React, { useEffect, useState } from "react";
import { getVillages } from "../../../api/Village";

import { useDispatch, useSelector } from "react-redux";
import MUITable from "../../ref/MUITable";
import ContentBox from "../../ContentBox";

const VillagesTable = () => {
	const account = useSelector((state) => state.account.info);
	const village = useSelector((state) => state.village.info);
	const [villages, setVillages] = useState([
		{ id: 0, name: "Loading ...", coords: { x: 0, y: 0 } },
	]);
	const [selected, setSelected] = useState(village.id || 0);
	const dispatch = useDispatch();

	const onClick = (vill) => {
		setSelected(vill.id);
	};

	useEffect(() => {
		if (account.id !== -1) {
			getVillages(account.id).then((data) => setVillages(data));
		}
	}, [selected, dispatch, account.id]);

	const header = ["Name", "Coords"];

	return (
		<>
			<ContentBox>
				<MUITable
					header={header}
					data={villages}
					handler={onClick}
					selected={selected}
				/>
			</ContentBox>
		</>
	);
};

export default VillagesTable;
