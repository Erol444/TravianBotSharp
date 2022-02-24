import * as yup from "yup";

const ChromeSchema = yup
	.object()
	.shape({
		click: yup.object().shape({
			min: yup
				.number()
				.min(1)
				.integer()
				.required("Minimum is required")
				.typeError("Minimum must be a number"),
			max: yup
				.number()
				.moreThan(
					yup.ref("min"),
					`Maximum must be greater than minimum`
				)
				.integer()
				.required("Maximum is required")
				.typeError("Maximum must be a number"),
		}),
	})
	.required();

export default ChromeSchema;
